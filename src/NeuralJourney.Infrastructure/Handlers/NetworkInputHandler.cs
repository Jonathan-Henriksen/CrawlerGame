using NeuralJourney.Core.Exceptions;
using NeuralJourney.Core.Interfaces.Handlers;
using NeuralJourney.Core.Interfaces.Services;
using Serilog;
using System.Net.Sockets;

namespace NeuralJourney.Infrastructure.Handlers
{
    public class NetworkInputHandler : IInputHandler<TcpClient>
    {
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
                throw new InvalidOperationException("Could not read from stream");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (!stream.DataAvailable)
                    {
                        await Task.Delay(200, cancellationToken);
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
                    return;
                }
                catch (MessageException ex)
                {
                    if (!client.Connected)
                        return;

                    await _messageService.SendMessageAsync(client, ex.Message, cancellationToken);

                    await Task.Delay(500, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Unexpected error while handling network input");

                    await _messageService.SendCloseConnectionAsync(client, cancellationToken);

                    client.Close();
                    client.Dispose();

                    return;
                }
            }
        }
    }
}