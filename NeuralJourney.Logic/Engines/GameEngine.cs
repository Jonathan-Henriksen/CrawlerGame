using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Commands;
using NeuralJourney.Logic.Handlers;
using NeuralJourney.Logic.Services;

namespace NeuralJourney.Logic.Engines
{
    public class GameEngine : IGameEngine
    {
        private readonly IClockService _clock;

        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IInputHandler _inputHandler;
        private readonly IConnectionHandler _connectionHandler;

        private CancellationTokenSource? _adminInputToken;
        private readonly Dictionary<Player, CancellationTokenSource> _playerTokens = new();

        private readonly List<Player> Players = new();

        public GameEngine(IClockService clockService, ICommandDispatcher commandDispatcher, IConnectionHandler connectionHandler, IInputHandler inputHandler)
        {
            _clock = clockService;
            _commandDispatcher = commandDispatcher;
            _connectionHandler = connectionHandler;
            _inputHandler = inputHandler;

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
        }

        private void AddPlayer(Player player)
        {
            var cts = new CancellationTokenSource();

            if (!_playerTokens.TryAdd(player, cts))
                throw new InvalidOperationException("Could not add player");

            _inputHandler.HandlePlayerInputAsync(player, cts.Token);
            Players.Add(player);
        }

        private void RemovePlayer(Player player)
        {
            if (_playerTokens.TryGetValue(player, out var cts))
            {
                cts.Cancel();
                _playerTokens.Remove(player);
            }

            Players.Remove(player);
        }
    }
}