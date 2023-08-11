using NeuralJourney.Core.Interfaces.Handlers;
using NeuralJourney.Core.Interfaces.Services;
using Serilog;
using System.Net.Sockets;

namespace NeuralJourney.Infrastructure.Handlers
{
    public class NetworkInputHandler : IInputHandler<NetworkStream>
    {
        private const int MaxReconnectionAttempts = 3;

        private readonly IMessageService _messageService;
        private readonly ILogger _logger;

        private CancellationTokenSource _tokenSource;

        public event Action<string, NetworkStream>? OnInputReceived;
        public event Action<NetworkStream>? OnClosedConnection;

        public NetworkInputHandler(IMessageService messageService, ILogger logger)
        {
            _messageService = messageService;
            _logger = logger;

            _tokenSource = new CancellationTokenSource();
        }

        public async Task HandleInputAsync(NetworkStream stream, CancellationToken cancellationToken = default)
        {
            if (!stream.CanRead)
                throw new InvalidOperationException("Failed to handle stream input. Reason: Could not read from stream");

            if (cancellationToken != default)
                _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var reconnectionAttempts = 0;

            while (!_tokenSource.IsCancellationRequested)
            {
                try
                {
                    var input = await _messageService.ReadMessageAsync(stream, _tokenSource.Token);

                    if (_messageService.IsCloseConnectionMessage(input))
                    {
                        OnClosedConnection?.Invoke(stream);
                        return;
                    }

                    _tokenSource.Token.ThrowIfCancellationRequested();

                    if (string.IsNullOrEmpty(input))
                        continue;

                    OnInputReceived?.Invoke(input, stream);
                }
                catch (IOException ex)
                {
                    if (ex.InnerException is SocketException socketEx && socketEx.SocketErrorCode == SocketError.ConnectionReset)
                    {
                        _logger.Warning("An existing connection was forcibly closed by the remote host: {Host}", stream.Socket.RemoteEndPoint);
                        return;
                    }

                    _logger.Warning("Network input stream encountered an error: {ErrorMessage}. Attempting to reconnect...", ex.Message);
                    reconnectionAttempts++;

                    if (reconnectionAttempts >= MaxReconnectionAttempts)
                    {
                        _logger.Error("Max reconnection attempts({Limit}) reached. Shutting down the client", MaxReconnectionAttempts);
                        return;
                    }

                    await Task.Delay(3000, _tokenSource.Token);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, ex.Message);
                    if (stream.CanWrite)
                        await _messageService.SendCloseConnectionAsync(stream, _tokenSource.Token);
                }
            }
        }

        public void Dispose()
        {
            _tokenSource.Cancel();
            _tokenSource.Dispose();
        }
    }
}