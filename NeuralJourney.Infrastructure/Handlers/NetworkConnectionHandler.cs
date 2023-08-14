﻿using NeuralJourney.Core.Constants.Messages;
using NeuralJourney.Core.Interfaces.Handlers;
using NeuralJourney.Core.Options;
using Serilog;
using System.Net;
using System.Net.Sockets;

namespace NeuralJourney.Infrastructure.Handlers
{
    public class NetworkConnectionHandler : IConnectionHandler
    {
        private const int MaxRetryAttempts = 5;

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
                    return; // Return back to caller who initialized cancellation
                }
                catch (SocketException socketException) when (socketException.SocketErrorCode == SocketError.OperationAborted)
                {
                    _logger.Warning(socketException, "A client disconnected unexpectedly. Reason: {Message}", socketException.Message);
                    retryCount++;

                    continue; // Recover when a single client disconnects unexpectedly
                }
                catch (SocketException ex)
                {
                    if (retryCount++ > MaxRetryAttempts)
                    {
                        // TODO: Add Logging
                        return;
                    }

                    _logger.Warning(ex, "Network Error: {ServiceName} encountered a connection issue. Retrying {RetryAttemptCount}/{RetryAttemptLimit}", nameof(NetworkConnectionHandler), retryCount, MaxRetryAttempts);

                    await Task.Delay(3000, cancellationToken); // Wait for 3 seconds before retrying

                    continue;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "NetworkConnectionHandler encountered an unexpected error. Message: {Message}", ex.Message);
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
