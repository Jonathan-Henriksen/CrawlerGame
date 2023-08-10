using NeuralJourney.Logic.Handlers.Interfaces;
using NeuralJourney.Logic.Services.Interfaces;
using Serilog;
using System.Net.Sockets;

namespace NeuralJourney.Logic.Handlers
{
    public class NetworkInputHandler : IInputHandler<NetworkStream>
    {
        private const int MaxReconnectionAttempts = 3;

        private readonly IMessageService _messageService;
        private readonly ILogger _logger;

        public event Action<string, NetworkStream>? OnInputReceived;
        public event Action<NetworkStream>? OnClosedConnection;

        public NetworkInputHandler(IMessageService messageService, ILogger logger)
        {
            _messageService = messageService;
            _logger = logger;
        }

        public async Task HandleInputAsync(NetworkStream stream, CancellationToken cancellationToken = default)
        {
            if (!stream.CanRead)
                throw new InvalidOperationException("Failed to handle stream input. Reason: Cannot read from stream");

            var clientCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            int reconnectionAttempts = 0;

            while (!clientCts.IsCancellationRequested && reconnectionAttempts < MaxReconnectionAttempts)
            {
                try
                {
                    var input = await _messageService.ReadMessageAsync(stream, clientCts.Token);
                    if (_messageService.IsCloseConnectionMessage(input))
                    {
                        OnClosedConnection?.Invoke(stream);
                        await stream.DisposeAsync();

                        return;
                    }

                    clientCts.Token.ThrowIfCancellationRequested();

                    if (string.IsNullOrEmpty(input))
                        continue;

                    OnInputReceived?.Invoke(input, stream);
                }
                catch (IOException ex)
                {
                    _logger.Warning("Network input stream encountered an error: {ErrorMessage}. Attempting to reconnect...", ex.Message);

                    reconnectionAttempts++;

                    await Task.Delay(3000, clientCts.Token);

                    clientCts.Token.ThrowIfCancellationRequested();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, ex.Message);

                    if (stream.CanWrite)
                    {
                        await _messageService.SendMessageAsync(stream, "Encounted an unexpected error. Closing connection...", clientCts.Token);
                        await _messageService.SendCloseConnectionAsync(stream, clientCts.Token);
                    }

                    clientCts.Cancel();
                    clientCts.Dispose();

                    await stream.DisposeAsync();
                }
            }

            if (reconnectionAttempts == MaxReconnectionAttempts)
            {
                _logger.Error("Max reconnection attempts reached. Shutting down the network handler");

                clientCts.Cancel();
                clientCts.Dispose();

                await stream.DisposeAsync();
            }
        }
    }
}