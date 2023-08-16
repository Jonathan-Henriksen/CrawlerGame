using NeuralJourney.Core.Constants;
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

            _logger.Debug(NetworkLogTemplates.Debug.TcpListenerStarted);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var client = await _tcpListener.AcceptTcpClientAsync(cancellationToken);

                    if (client is null)
                        continue;

                    _logger.Information(NetworkLogTemplates.Info.ClientConnected, client.Client.RemoteEndPoint);
                    OnConnected?.Invoke(client);

                    retryCount = 0; // Reset retry count on successful connection
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (SocketException ex)
                {
                    if (++retryCount > _maxRetryAttempts)
                    {
                        _logger.Error(ex, NetworkLogTemplates.Error.SocketFailure);
                        throw new OperationCanceledException();
                    }

                    _logger.Warning(ex, NetworkLogTemplates.Warning.SocketFailureRetry, retryCount, _maxRetryAttempts);

                    await Task.Delay(5000, cancellationToken); // Give connection some time to recover

                    continue;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, NetworkLogTemplates.Error.UnexpectedError);

                    throw;
                }
            }
        }

        public void Dispose()
        {
            _tcpListener.Stop();
            _tcpListener.Server.Dispose();

            _logger.Debug(SystemMessageTemplates.DispoedOfType, GetType().Name);
        }
    }
}
