using NeuralJourney.Library.Constants;
using NeuralJourney.Logic.Engines.Interfaces;
using NeuralJourney.Logic.Handlers.Interfaces;
using NeuralJourney.Logic.Services.Interfaces;
using Serilog;

namespace NeuralJourney.Logic.Engines
{
    public class ServerEngine : IEngine
    {
        private readonly IClockService _clock;
        private readonly ILogger _logger;

        private readonly IConnectionHandler _connectionHandler;
        private readonly IPlayerHandler _playerHandler;

        public ServerEngine(IClockService clockService, ILogger logger, IConnectionHandler connectionHandler, IPlayerHandler playerHandler)
        {
            _clock = clockService;
            _logger = logger;

            _connectionHandler = connectionHandler;
            _playerHandler = playerHandler;

            _connectionHandler.OnConnected += _playerHandler.AddPlayer;
        }

        public async Task<IEngine> Init(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(this);
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            _clock.Start();

            var connectionHandlerTask = _connectionHandler.HandleConnectionsAsync();

            _logger.Information(InfoMessageTemplates.GameStarted);

            await connectionHandlerTask;
        }

        public Task Stop()
        {
            return Task.Run(() =>
            {
                _connectionHandler.Stop();

                _clock.Reset();

                _logger.Information(InfoMessageTemplates.GameStopped);
            });
        }
    }
}