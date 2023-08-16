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

            _logger.Information(InfoMessageTemplates.GameStarted);

            _clock.Start();

            try
            {
                await _connectionHandler.HandleConnectionsAsync(_token);
            }
            catch (OperationCanceledException)
            {
                await Stop();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "The server engine encountered an error");
            }
        }

        public Task Stop()
        {
            _logger.Information(InfoMessageTemplates.GameStopped);
            _clock.Reset();

            return Task.CompletedTask;
        }

        private void AcceptConnections(TcpClient client)
        {
            _playerHandler.AddPlayer(client, _token);
        }

        public void Dispose()
        {
            _logger.Debug(DebugMessageTemplates.DispoedOfType, GetType().Name);
        }
    }
}