using NeuralJourney.Core.Constants;
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

        private CancellationToken _token;

        public ServerEngine(IClockService clockService, ILogger logger, IConnectionHandler connectionHandler, IPlayerHandler playerHandler)
        {
            _clock = clockService;
            _logger = logger.ForContext<ServerEngine>();
            _connectionHandler = connectionHandler;
            _playerHandler = playerHandler;

            _connectionHandler.OnConnected += AcceptConnections;
        }

        public async Task Run(CancellationToken cancellationToken = default)
        {

            _token = cancellationToken;

            _logger.Information(ServerLogTemplates.Info.ServerStarted);

            _clock.Start();

            try
            {
                await _connectionHandler.HandleConnectionsAsync(_token);
            }
            catch (OperationCanceledException)
            {
                return; // Engine is stopped before cancelling token to ensure players connections are closed
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ServerLogTemplates.Error.UnexpectedError);
            }
        }

        public async Task StopAsync()
        {
            _logger.Information(ServerLogTemplates.Info.ServerStopped);

            await _playerHandler.RemoveAllPlayers();

            _clock.Reset();
        }

        private void AcceptConnections(TcpClient client)
        {
            _playerHandler.AddPlayer(client, _token);
        }

        public void Dispose()
        {
            _logger.Debug(SystemMessageTemplates.DispoedOfType, GetType().Name);
        }
    }
}