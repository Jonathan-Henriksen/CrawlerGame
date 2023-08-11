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
            _inputHandler.OnClosedConnection += RemovePlayer;
        }

        public void AddPlayer(TcpClient playerClient, CancellationToken cancellationToken = default)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var player = new Player(playerClient);

            if (!_players.TryAdd(player, cts))
                throw new InvalidOperationException($"Failed to add player '{player.Name}' to the game. Reason: Another player with same name is already connected.");

            _ = _inputHandler.HandleInputAsync(player, cts.Token); // Start background task to dispatch commands on input
        }

        private void DispatchCommand(string input, Player player)
        {
            var context = new CommandContext(input, CommandTypeEnum.Player, player);
            _commandDispatcher.DispatchCommand(context);
        }

        private void RemovePlayer(Player player)
        {
            if (!_players.TryGetValue(player, out var cts))
                return;

            _inputHandler.OnInputReceived -= DispatchCommand;
            _inputHandler.OnClosedConnection -= RemovePlayer;

            cts.Cancel();
            cts.Dispose();

            var stream = player.GetStream();
            stream.Socket.Close();
            stream.Socket.Dispose();

            _players.Remove(player);

            _logger.Information("{PlayerName} left the gaeme", player.Name);
        }
    }
}