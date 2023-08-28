using NeuralJourney.Core.Constants;
using NeuralJourney.Core.Extensions;
using NeuralJourney.Core.Interfaces.Handlers;
using NeuralJourney.Core.Models.Options;
using Serilog;
using System.Net;
using System.Net.Sockets;

namespace NeuralJourney.Infrastructure.Handlers
{
    public class NetworkConnectionHandler : IConnectionHandler
    {
        private const int _maxRetryAttempts = 10;
        private int RetryAttempts = 0;

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

            _logger.Debug(NetworkLogMessages.Debug.TcpListenerStarted);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var client = await _tcpListener.AcceptTcpClientAsync(cancellationToken);

                    if (client is null)
                        continue;

                    _logger.Debug(NetworkLogMessages.Debug.ClientConnected, client.GetRemoteIp());
                    OnConnected?.Invoke(client);

                    RetryAttempts = 0; // Reset retry count on successful connection
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (SocketException ex)
                {
                    if (++RetryAttempts > _maxRetryAttempts)
                    {
                        _logger.Error(ex, NetworkLogMessages.Error.SocketFailure);
                        throw new OperationCanceledException();
                    }

                    _logger.Warning(ex, NetworkLogMessages.Warning.SocketFailureRetry, RetryAttempts, _maxRetryAttempts);

                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, RetryAttempts)), cancellationToken); // Exponential back-off

                    continue;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, NetworkLogMessages.Error.UnexpectedError);

                    throw;
                }
            }
        }

        public void Dispose()
        {
            _tcpListener.Stop();
            _tcpListener.Server.Dispose();

            _logger.Debug(SystemMessages.DispoedOfType, GetType().Name);
        }
    }
}
