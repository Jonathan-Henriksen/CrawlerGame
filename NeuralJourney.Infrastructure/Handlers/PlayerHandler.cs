using NeuralJourney.Core.Enums.Commands;
using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Interfaces.Handlers;
using NeuralJourney.Core.Models.Commands;
using NeuralJourney.Core.Models.World;
using Serilog;
using System.Net.Sockets;

namespace NeuralJourney.Infrastructure.Handlers
{
    public class PlayerHandler : IPlayerHandler, IDisposable
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

            cts.Cancel();
            cts.Dispose();

            var stream = player.GetStream();
            stream.Socket.Close();
            stream.Socket.Dispose();

            _players.Remove(player);

            _logger.Information("{PlayerName} left the gaeme", player.Name);
        }

        public void Dispose()
        {
            _logger.Debug("Disposing of {Type}", GetType().Name);

            _inputHandler.OnInputReceived -= DispatchCommand;
            _inputHandler.OnClosedConnection -= RemovePlayer;

            foreach (var player in _players.Keys)
            {
                RemovePlayer(player);
            }

            _players.Clear();
        }
    }
}