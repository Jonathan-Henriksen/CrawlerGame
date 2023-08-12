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
            _logger = logger;
        }

        public async Task HandleInputAsync(Player player, CancellationToken cancellationToken)
        {
            var stream = player.GetStream();

            var reconnectionAttempts = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var input = await _messageService.ReadMessageAsync(stream, cancellationToken);

                    if (_messageService.IsCloseConnectionMessage(input))
                    {
                        OnClosedConnection?.Invoke(player);
                        return;
                    }

                    if (string.IsNullOrEmpty(input))
                        continue;

                    OnInputReceived?.Invoke(input, player);

                    reconnectionAttempts = 0; // Reset reconnection attempts on successful input
                }
                catch (IOException ex)
                {
                    if (ex.InnerException is SocketException socketEx && socketEx.SocketErrorCode == SocketError.ConnectionReset)
                    {
                        _logger.Information(InfoMessageTemplates.ClientDisconnected, stream.Socket.RemoteEndPoint);
                        return;
                    }

                    _logger.Warning("Lost connection to ({PlayerName}). Waiting for reconnect: {ErrorMessage}", player.Name, ex.Message);
                    reconnectionAttempts++;

                    if (reconnectionAttempts >= MaxReconnectionAttempts)
                    {
                        _logger.Error("({PlayerName}) reached the reconnection attempt limit({Limit}) and was removed from the game", player.Name, MaxReconnectionAttempts);
                        return;
                    }

                    await Task.Delay(3000, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    throw; // Thorw back to player handler
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Unexpected error while reading the stream of ({PlayerName}): {ErrorMessage}", player.Name, ex.Message);
                }
            }
        }
    }
}