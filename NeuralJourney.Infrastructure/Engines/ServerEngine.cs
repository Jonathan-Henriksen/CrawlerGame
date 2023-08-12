using NeuralJourney.Core.Constants.Messages;
using NeuralJourney.Core.Interfaces.Engines;
using NeuralJourney.Core.Interfaces.Handlers;
using NeuralJourney.Core.Interfaces.Services;
using Serilog;
using System.Net.Sockets;

namespace NeuralJourney.Infrastructure.Engines
{
    public class ServerEngine : IEngine
    {
        private readonly IClockService _clock;
        private readonly ILogger _logger;
        private readonly IConnectionHandler _connectionHandler;
        private readonly IPlayerHandler _playerHandler;

        private CancellationTokenSource _tokenSource;

        public ServerEngine(IClockService clockService, ILogger logger, IConnectionHandler connectionHandler, IPlayerHandler playerHandler)
        {
            _clock = clockService;
            _logger = logger;
            _connectionHandler = connectionHandler;
            _playerHandler = playerHandler;

            _tokenSource = new CancellationTokenSource();

            _connectionHandler.OnConnected += AcceptConnections;
        }

        public async Task Run(CancellationToken cancellationToken = default)
        {
            if (cancellationToken != default)
                _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _clock.Start();

            _logger.Information(InfoMessageTemplates.GameStarted);

            try
            {
                await _connectionHandler.HandleConnectionsAsync(_tokenSource.Token);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected error occcured in {Type}", GetType().Name);
            }
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
            _playerHandler.AddPlayer(client, _tokenSource.Token);
        }

        public void Dispose()
        {
            if (!_tokenSource.IsCancellationRequested)
                _tokenSource.Cancel();

            _connectionHandler.Dispose();
            _playerHandler.Dispose();

            _tokenSource.Dispose();

            _logger.Debug(DebugMessageTemplates.DispoedOfType, GetType().Name);
        }
    }
}