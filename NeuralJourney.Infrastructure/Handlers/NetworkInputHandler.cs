using NeuralJourney.Core.Interfaces.Handlers;
using NeuralJourney.Core.Interfaces.Services;
using Serilog;
using System.Net.Sockets;

namespace NeuralJourney.Infrastructure.Handlers
{
    public class NetworkInputHandler : IInputHandler<TcpClient>
    {
        private const int _maxReconnectionAttempts = 3;

        private readonly IMessageService _messageService;
        private readonly ILogger _logger;

        public event Action<string, TcpClient>? OnInputReceived;
        public event Action<TcpClient>? OnClosedConnection;

        public NetworkInputHandler(IMessageService messageService, ILogger logger)
        {
            _messageService = messageService;
            _logger = logger.ForContext<NetworkInputHandler>();
        }

        public async Task HandleInputAsync(TcpClient client, CancellationToken cancellationToken)
        {
            var stream = client.GetStream();

            if (!stream.CanRead)
                throw new InvalidOperationException("Failed to handle stream input. Reason: Could not read from stream");

            var reconnectionAttempts = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (!stream.DataAvailable)
                    {
                        await Task.Delay(200);
                        continue;
                    }

                    var input = await _messageService.ReadMessageAsync(client, cancellationToken);

                    cancellationToken.ThrowIfCancellationRequested();

                    if (_messageService.IsCloseConnectionMessage(input))
                    {
                        OnClosedConnection?.Invoke(client);
                        return;
                    }

                    if (string.IsNullOrEmpty(input))
                        continue;

                    OnInputReceived?.Invoke(input, client);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (IOException ex)
                {
                    if (ex.InnerException is SocketException socketEx && socketEx.SocketErrorCode == SocketError.ConnectionReset)
                    {
                        _logger.Debug("An existing connection was forcibly closed by the remote host: {Host}", stream.Socket.RemoteEndPoint);
                        _logger.Information("The server closed the connection");
                        return;
                    }

                    _logger.Warning("Network input stream encountered an error: {ErrorMessage}. Attempting to reconnect...", ex.Message);
                    reconnectionAttempts++;

                    if (reconnectionAttempts >= _maxReconnectionAttempts)
                    {
                        _logger.Error("Max reconnection attempts({Limit}) reached. Shutting down the client", _maxReconnectionAttempts);
                        return;
                    }

                    await Task.Delay(100, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, ex.Message);
                    if (stream.CanWrite)
                        await _messageService.SendCloseConnectionAsync(client, cancellationToken);
                }
            }
        }
    }
}