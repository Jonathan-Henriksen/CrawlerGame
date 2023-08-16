using NeuralJourney.Core.Constants.Messages;
using NeuralJourney.Core.Interfaces.Handlers;
using NeuralJourney.Core.Options;
using Serilog;
using System.Net;
using System.Net.Sockets;

namespace NeuralJourney.Infrastructure.Handlers
{
    public class NetworkConnectionHandler : IConnectionHandler
    {
        private const int _maxRetryAttempts = 10;

        private readonly TcpListener _tcpListener;
        private readonly ILogger _logger;

        public event Action<TcpClient>? OnConnected;

        public NetworkConnectionHandler(NetworkOptions networkOptions, ILogger logger)
        {
            _tcpListener = new TcpListener(IPAddress.Any, networkOptions.Port);
            _logger = logger.ForContext<NetworkConnectionHandler>();
        }

        public async Task HandleConnectionsAsync(CancellationToken cancellationToken)
        {
            _tcpListener.Start();

            var retryCount = 0;

            _logger.Information(InfoMessageTemplates.ServerStarted);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var client = await _tcpListener.AcceptTcpClientAsync(cancellationToken);

                    if (client is null)
                        continue;

                    _logger.Information(InfoMessageTemplates.ClientConnected, client.Client.RemoteEndPoint);
                    OnConnected?.Invoke(client);

                    retryCount = 0; // Reset retry count on successful connection
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (SocketException ex)
                {
                    ++retryCount;

                    if (retryCount > _maxRetryAttempts)
                    {
                        _logger.Error(ex, "Failed to accept incoming connections. Retry limit reached");
                        throw new OperationCanceledException();
                    }

                    _logger.Warning(ex, "Error while listening for incoming connections {RetryCount}/{RetryLimit}", retryCount, _maxRetryAttempts);

                    await Task.Delay(5000, cancellationToken); // Give connection some time to recover

                    continue;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Unexpected error while handling network input");

                    throw;
                }
            }
        }

        public void Dispose()
        {
            _tcpListener.Stop();
            _tcpListener.Server.Dispose();

            _logger.Debug(DebugMessageTemplates.DispoedOfType, GetType().Name);
        }
    }
}
