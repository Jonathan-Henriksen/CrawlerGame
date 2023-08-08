using NeuralJourney.Library.Constants;
using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Commands;
using NeuralJourney.Logic.Handlers;
using NeuralJourney.Logic.Services;
using Serilog;

namespace NeuralJourney.Logic.Engines
{
    public class GameEngine : IGameEngine
    {
        private readonly IClockService _clock;

        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IConnectionHandler _connectionHandler;
        private readonly IInputHandler _inputHandler;
        private readonly ILogger _logger;

        private CancellationTokenSource? _adminInputToken;
        private readonly Dictionary<Player, CancellationTokenSource> _playerTokens = new();

        private readonly List<Player> Players = new();

        public GameEngine(IClockService clockService, ICommandDispatcher commandDispatcher, IConnectionHandler connectionHandler, IInputHandler inputHandler, ILogger logger)
        {
            _clock = clockService;
            _commandDispatcher = commandDispatcher;
            _connectionHandler = connectionHandler;
            _inputHandler = inputHandler;
            _logger = logger;

            _inputHandler.OnAdminInputReceived += _commandDispatcher.DispatchAdminCommand;
            _inputHandler.OnPlayerInputReceived += _commandDispatcher.DispatchPlayerCommand;

            _connectionHandler.OnPlayerConnected += AddPlayer;
        }

        public async Task Run()
        {
            _clock.Start();

            _adminInputToken = new CancellationTokenSource();

            var connectionHandlerTask = _connectionHandler.HandleConnectionsAsync();
            var inputHandlerTask = _inputHandler.HandleAdminInputAsync(_adminInputToken.Token);

            _logger.Information(InfoMessageTemplates.GameStarted);

            await Task.WhenAll(connectionHandlerTask, inputHandlerTask);
        }

        public void Stop()
        {
            _adminInputToken?.Cancel();

            _connectionHandler.Stop();

            foreach (var player in Players)
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
                throw new InvalidOperationException("Failed to add player '{0}' to the game. A cancellation token is alread associated with the player.");

            _inputHandler.HandlePlayerInputAsync(player, cts.Token);
            Players.Add(player);

            _logger.Information(InfoMessageTemplates.PlayerAdded, player.Name);
        }

        private void RemovePlayer(Player player)
        {
            if (_playerTokens.TryGetValue(player, out var cts))
            {
                cts.Cancel();
                _playerTokens.Remove(player);
            }

            Players.Remove(player);

            _logger.Information(InfoMessageTemplates.PlayerRemoved, player.Name);
        }
    }
}