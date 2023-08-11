using NeuralJourney.Library.Constants;
using NeuralJourney.Logic.Handlers.Interfaces;
using NeuralJourney.Logic.Options;
using Serilog;
using System.Net;
using System.Net.Sockets;

namespace NeuralJourney.Logic.Handlers
{
    public class NetworkConnectionHandler : IConnectionHandler
    {
        private const int MaxRetryAttempts = 5;
        private CancellationTokenSource _cts;
        private readonly TcpListener _tcpListener;
        private readonly ILogger _logger;

        public event Action<TcpClient>? OnConnected;

        public NetworkConnectionHandler(ServerOptions serverOptions, ILogger logger)
        {
            _cts = new CancellationTokenSource();
            _tcpListener = new TcpListener(IPAddress.Any, serverOptions.Port);
            _logger = logger;
        }

        public async Task HandleConnectionsAsync(CancellationToken cancellationToken = default)
        {
            _tcpListener.Start();

            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            int retryCount = 0;

            _logger.Information(InfoMessageTemplates.ServerStarted);

            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    var client = await _tcpListener.AcceptTcpClientAsync(_cts.Token);

                    if (client is null)
                        continue;

                    _logger.Information(InfoMessageTemplates.ClientConnected, client.Client.RemoteEndPoint);
                    OnConnected?.Invoke(client);

                    retryCount = 0; // Reset retry count on successful connection
                }
                catch (OperationCanceledException)
                {
                    // Expected exception when cancellation is requested, no need to log.
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;

                    _logger.Warning(ex, "Error encountered while listening for clients. Attempt {RetryCount}/{MaxAttempts}.\nError: {ErrorMessage}", retryCount, MaxRetryAttempts, ex.Message);

                    if (retryCount > MaxRetryAttempts)
                    {

                        _logger.Error("Max retry attempts reached. Shutting down the server.");
                        break;
                    }

                    await Task.Delay(3000, cancellationToken); // Wait for 3 seconds before retrying
                }
            }

            Cleanup();
        }

        public void Stop()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            _tcpListener.Stop();
            _tcpListener.Server.Dispose();

            _cts.Cancel();
            _cts.Dispose();

            _logger.Information(InfoMessageTemplates.ServerStopped);
        }
    }
}
