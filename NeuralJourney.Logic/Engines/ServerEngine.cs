using NeuralJourney.Library.Constants;
using NeuralJourney.Logic.Engines.Interfaces;
using NeuralJourney.Logic.Handlers.Interfaces;
using NeuralJourney.Logic.Services.Interfaces;
using Serilog;
using System.Net.Sockets;

namespace NeuralJourney.Logic.Engines
{
    public class ServerEngine : IEngine, IDisposable
    {
        private readonly IClockService _clock;
        private readonly ILogger _logger;
        private readonly IConnectionHandler _connectionHandler;
        private readonly IPlayerHandler _playerHandler;

        private CancellationTokenSource _cts;

        public ServerEngine(IClockService clockService, ILogger logger, IConnectionHandler connectionHandler, IPlayerHandler playerHandler)
        {
            _clock = clockService;
            _logger = logger;
            _connectionHandler = connectionHandler;
            _playerHandler = playerHandler;

            _cts = new CancellationTokenSource();

            _connectionHandler.OnConnected += AcceptConnections;
        }

        public async Task Run(CancellationToken cancellationToken = default)
        {
            if (cancellationToken != default)
                _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _clock.Start();

            _logger.Information(InfoMessageTemplates.GameStarted);

            await _connectionHandler.HandleConnectionsAsync(_cts.Token);
        }

        public Task Stop()
        {
            return Task.Run(() =>
            {
                _clock.Reset();

                _logger.Information(InfoMessageTemplates.GameStopped);
            });
        }

        private void AcceptConnections(TcpClient client)
        {
            _playerHandler.HandlePlayer(client, _cts.Token);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connectionHandler.OnConnected -= AcceptConnections;

                _cts.Cancel();
                _cts.Dispose();
            }
        }
    }
}