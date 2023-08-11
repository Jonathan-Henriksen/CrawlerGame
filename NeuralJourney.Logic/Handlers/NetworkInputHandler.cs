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
                throw new InvalidOperationException("Failed to handle stream input. Reason: Could not read from stream");

            var clientCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            int reconnectionAttempts = 0;

            while (!clientCts.IsCancellationRequested)
            {
                try
                {
                    var input = await _messageService.ReadMessageAsync(stream, clientCts.Token);

                    if (_messageService.IsCloseConnectionMessage(input))
                    {
                        OnClosedConnection?.Invoke(stream);
                        Cleanup(stream, clientCts);
                        return;
                    }

                    clientCts.Token.ThrowIfCancellationRequested();

                    if (string.IsNullOrEmpty(input))
                        continue;

                    OnInputReceived?.Invoke(input, stream);
                }
                catch (IOException ex)
                {
                    if (ex.InnerException is SocketException socketEx && socketEx.SocketErrorCode == SocketError.ConnectionReset)
                    {
                        _logger.Warning("An existing connection was forcibly closed by the remote host: {Host}", stream.Socket.RemoteEndPoint);
                        Cleanup(stream, clientCts);
                        return;
                    }

                    _logger.Warning("Network input stream encountered an error: {ErrorMessage}. Attempting to reconnect...", ex.Message);
                    reconnectionAttempts++;

                    if (reconnectionAttempts >= MaxReconnectionAttempts)
                    {
                        _logger.Error("Max reconnection attempts({Limit}) reached. Shutting down the client", MaxReconnectionAttempts);
                        Cleanup(stream, clientCts);
                        return;
                    }

                    await Task.Delay(3000, clientCts.Token);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, ex.Message);
                    if (stream.CanWrite)
                    {
                        await _messageService.SendCloseConnectionAsync(stream, clientCts.Token);
                    }
                    Cleanup(stream, clientCts);
                }
            }
        }

        private static void Cleanup(NetworkStream stream, CancellationTokenSource clientCts)
        {
            clientCts.Cancel();
            clientCts.Dispose();
            stream.Close();
            stream.Socket.Dispose();
        }
    }
}
