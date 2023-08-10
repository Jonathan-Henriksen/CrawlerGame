using NeuralJourney.Library.Constants;
using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Commands.Interfaces;
using NeuralJourney.Logic.Engines.Interfaces;
using NeuralJourney.Logic.Handlers.Connection;
using NeuralJourney.Logic.Handlers.Input;
using NeuralJourney.Logic.Services.Interfaces;
using Serilog;

namespace NeuralJourney.Logic.Engines
{
    public class GameEngine : IGameEngine
    {
        private readonly IClockService _clock;

        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IConnectionHandler _connectionHandler;

        private readonly IInputHandler _adminInputHandler;
        private readonly IInputHandler _playerInputHandler;

        private readonly ILogger _logger;

        private readonly CancellationTokenSource _adminInputToken;

        private readonly List<Player> _players = new();
        private readonly Dictionary<Player, CancellationTokenSource> _playerTokens = new();

        public GameEngine(IClockService clockService, ICommandDispatcher commandDispatcher, IConnectionHandler connectionHandler, IEnumerable<IInputHandler> inputHandlers, ILogger logger)
        {
            _clock = clockService;
            _logger = logger;

            _adminInputToken = new CancellationTokenSource();

            _playerInputHandler = inputHandlers.First(i => i.GetType() == typeof(PlayerInputHandler));
            _adminInputHandler = inputHandlers.First(i => i.GetType() == typeof(AdminInputHandler));

            _commandDispatcher = commandDispatcher;

            _playerInputHandler.OnInputReceived += _commandDispatcher.DispatchCommand;
            _adminInputHandler.OnInputReceived += _commandDispatcher.DispatchCommand;

            _connectionHandler = connectionHandler;

            _connectionHandler.OnPlayerConnected += AddPlayer;
        }

        public async Task Run()
        {
            _clock.Start();

            var connectionHandlerTask = _connectionHandler.HandleConnectionsAsync();
            var inputHandlerTask = _playerInputHandler.HandleInputAsync(default, _adminInputToken.Token);

            _logger.Information(InfoMessageTemplates.GameStarted);

            await Task.WhenAll(connectionHandlerTask, inputHandlerTask);
        }

        public void Stop()
        {
            _adminInputToken.Cancel();

            _connectionHandler.Stop();

            foreach (var player in _players)
            {
                RemovePlayer(player);
            }

            _clock.Reset();

            _logger.Information(InfoMessageTemplates.GameStopped);
        }

        private void AddPlayer(Player player)
        {
            var cts = new CancellationTokenSource();

            if (!_playerTokens.TryAdd(player, cts))
                throw new InvalidOperationException("Failed to add player '{0}' to the game. Reason: A active cancellation token is already associated with the player.");

            _playerInputHandler.HandleInputAsync(player, cts.Token);
            _players.Add(player);

            _logger.Information(InfoMessageTemplates.PlayerAdded, player.Name);
        }

        private void RemovePlayer(Player player)
        {
            if (_playerTokens.TryGetValue(player, out var cts))
            {
                cts.Cancel();
                _playerTokens.Remove(player);
            }

            _players.Remove(player);

            _logger.Information(InfoMessageTemplates.PlayerRemoved, player.Name);
        }
    }
}