using NeuralJourney.Library.Constants;
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
        }

        public void AddPlayer(TcpClient playerClient)
        {
            var player = new Player(playerClient);

            var cts = new CancellationTokenSource();

            if (!_players.TryAdd(player, cts))
                throw new InvalidOperationException("Failed to add player '{0}' to the game. Reason: A active cancellation token is already associated with the player.");

            _ = _inputHandler.HandleInputAsync(player, cts.Token);

            _logger.Information(InfoMessageTemplates.PlayerAdded, player.Name);

        }

        private void DispatchCommand(string input, Player player)
        {
            var context = new CommandContext(input, CommandTypeEnum.Player, player);
            _commandDispatcher.DispatchCommand(context);
        }
    }
}