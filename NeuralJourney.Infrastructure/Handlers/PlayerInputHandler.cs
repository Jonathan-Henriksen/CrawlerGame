using NeuralJourney.Core.Constants.Messages;
using NeuralJourney.Core.Interfaces.Handlers;
using NeuralJourney.Core.Interfaces.Services;
using NeuralJourney.Core.Models.World;
using Serilog;
using System.Net.Sockets;

namespace NeuralJourney.Infrastructure.Handlers
{
    public class PlayerInputHandler : IInputHandler<Player>
    {
        private const int MaxReconnectionAttempts = 3;

        private readonly IMessageService _messageService;
        private readonly ILogger _logger;

        public event Action<string, Player>? OnInputReceived;
        public event Action<Player>? OnClosedConnection;

        public PlayerInputHandler(IMessageService messageService, ILogger logger)
        {
            _messageService = messageService;
            _logger = logger.ForContext<PlayerInputHandler>();
        }

        public async Task HandleInputAsync(Player player, CancellationToken cancellationToken)
        {
            var client = player.GetClient();

            var retryCount = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var input = await _messageService.ReadMessageAsync(client, cancellationToken);

                    retryCount = 0; // Reset after reading a message successfully

                    if (_messageService.IsCloseConnectionMessage(input))
                    {
                        OnClosedConnection?.Invoke(player);
                        return;
                    }

                    if (string.IsNullOrEmpty(input))
                        continue;

                    OnInputReceived?.Invoke(input, player);


                }
                catch (IOException ex)
                {
                    if (ex.InnerException is SocketException socketEx && socketEx.SocketErrorCode == SocketError.ConnectionReset)
                    {
                        _logger.Information(InfoMessageTemplates.ClientDisconnected, client.Client.RemoteEndPoint);
                        return;
                    }

                    _logger.Warning(ex, "Lost connection to player");

                    return;
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Unexpected error while reading from player stream {Address}. Retries: {RetryCount}/3", client.Client.RemoteEndPoint, retryCount);

                    if (retryCount < 3)
                    {
                        retryCount++;

                        if (client.Connected)
                            await _messageService.SendMessageAsync(client, $"Unexpected error while processing your message. Please try again ({retryCount + 1}/3)", cancellationToken);

                        continue;
                    }

                    await _messageService.SendMessageAsync(client, "Retry limit exceeded", cancellationToken);
                    await _messageService.SendCloseConnectionAsync(client, cancellationToken);

                    return;
                }
            }
        }
    }
}