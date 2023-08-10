using NeuralJourney.Library.Constants;
using NeuralJourney.Library.Enums.Commands;
using NeuralJourney.Library.Models.Commands;
using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Commands.Interfaces;
using NeuralJourney.Logic.Handlers.Interfaces;
using NeuralJourney.Logic.Services.Interfaces;
using Serilog;
using System.Net.Sockets;

namespace NeuralJourney.Logic.Handlers
{
    public class PlayerHandler : IPlayerHandler
    {
        private const int MaxReconnectionAttempts = 3;

        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IInputHandler<Player> _inputHandler;
        private readonly IMessageService _messageService;
        private readonly ILogger _logger;

        private readonly Dictionary<Player, CancellationTokenSource> _players = new();

        public PlayerHandler(ICommandDispatcher commandDispatcher, IInputHandler<Player> inputHandler, IMessageService messageService, ILogger logger)
        {
            _commandDispatcher = commandDispatcher;
            _inputHandler = inputHandler;
            _messageService = messageService;
            _logger = logger;

            _inputHandler.OnInputReceived += DispatchCommand;
        }

        public async void AddPlayer(TcpClient playerClient, CancellationToken cancellationToken = default)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var player = new Player(playerClient);

            if (!_players.TryAdd(player, cts))
                throw new InvalidOperationException($"Failed to add player '{player.Name}' to the game. Reason: Another player with same name is already connected.");

            int reconnectionAttempts = 0;

            while (!cts.IsCancellationRequested)
            {
                if (reconnectionAttempts == MaxReconnectionAttempts) // Max reconnect attempts reached
                {
                    _logger.Error("{PlayerName} reached max reconnection attempts. Removing player from the game.", player.Name);

                    RemovePlayer(player);

                    return;
                }

                try
                {
                    await _inputHandler.HandleInputAsync(player, cts.Token); // Start background task to dispatch commands when input is received

                    reconnectionAttempts = 0;
                }
                catch (IOException ex)
                {
                    if (ex.InnerException is SocketException socketEx && socketEx.SocketErrorCode == SocketError.ConnectionReset) // An existing connection was forcibly closed by the remote host
                    {
                        _logger.Information(InfoMessageTemplates.ClientDisconnected, player.GetStream().Socket.RemoteEndPoint);
                        RemovePlayer(player);
                        return;
                    }

                    _logger.Warning("{PlayerName}: {ErrorMessage}. Attempting to reconnect...", player.Name, ex.Message);

                    reconnectionAttempts++;
                    await Task.Delay(3000, cts.Token);
                }
                catch (Exception ex) // Gracefully disconnect and shut down on unexpected errors
                {
                    _logger.Error(ex, ex.Message);

                    var stream = playerClient.GetStream();

                    await _messageService.SendMessageAsync(stream, "Encounted an unexpected error. Closing connection...", cts.Token);
                    await _messageService.SendCloseConnectionAsync(stream, cts.Token);

                    RemovePlayer(player);

                    return;
                }
            }
        }

        private void DispatchCommand(string input, Player player)
        {
            if (_messageService.IsCloseConnectionMessage(input)) // Handle graceful disconnect from remote
            {
                _logger.Information("Player {PlayerName} disconnected", player.Name);
                RemovePlayer(player);

                return;
            }

            var context = new CommandContext(input, CommandTypeEnum.Player, player);
            _commandDispatcher.DispatchCommand(context);
        }

        private void RemovePlayer(Player player)
        {
            if (!_players.TryGetValue(player, out var cts))
                return;
            var stream = player.GetStream();

            cts.Cancel();
            cts.Dispose();

            stream.Socket.Close();
            stream.Socket.Dispose();

            _players.Remove(player);
        }
    }
}