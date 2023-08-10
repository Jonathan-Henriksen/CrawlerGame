using NeuralJourney.Library.Enums.Commands;
using NeuralJourney.Library.Models.Commands;
using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Commands.Interfaces;
using NeuralJourney.Logic.Handlers.Interfaces;
using Serilog;
using System.Net.Sockets;

namespace NeuralJourney.Logic.Handlers
{
    public class PlayerHandler : IPlayerHandler
    {
        private const int MaxReconnectionAttempts = 3;

        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IInputHandler<Player> _inputHandler;
        private readonly ILogger _logger;

        private readonly Dictionary<Player, CancellationTokenSource> _players = new();

        public PlayerHandler(ICommandDispatcher commandDispatcher, IInputHandler<Player> inputHandler, ILogger logger)
        {
            _commandDispatcher = commandDispatcher;
            _inputHandler = inputHandler;
            _logger = logger;

            _inputHandler.OnInputReceived += DispatchCommand;
        }

        public async void AddPlayer(TcpClient playerClient, CancellationToken cancellationToken = default)
        {
            var playerCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var player = new Player(playerClient);

            if (!_players.TryAdd(player, playerCts))
                throw new InvalidOperationException($"Failed to add player '{player.Name}' to the game. Reason: An active cancellation token is already associated with the player.");

            int reconnectionAttempts = 0;

            while (!playerCts.IsCancellationRequested && reconnectionAttempts < MaxReconnectionAttempts)
            {
                try
                {
                    await _inputHandler.HandleInputAsync(player, playerCts.Token);
                    reconnectionAttempts = 0;
                }
                catch (Exception ex)
                {
                    _logger.Warning("Error handling player {PlayerName}: {ErrorMessage}. Attempting to reconnect...", player.Name, ex.Message);

                    reconnectionAttempts++;
                    await Task.Delay(3000, playerCts.Token);
                }
            }

            if (reconnectionAttempts == MaxReconnectionAttempts)
                _logger.Error("{PlayerName} reached max reconnection attempts. Removing player from the game.", player.Name);

            RemovePlayer(player);
        }

        private void DispatchCommand(string input, Player player)
        {
            var context = new CommandContext(input, CommandTypeEnum.Player, player);
            _commandDispatcher.DispatchCommand(context);
        }

        private void RemovePlayer(Player player)
        {
            if (!_players.TryGetValue(player, out var playerCts))
                return;

            playerCts.Cancel();
            playerCts.Dispose();

            player.GetStream().Dispose();

            _players.Remove(player);
        }
    }
}