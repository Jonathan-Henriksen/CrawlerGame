﻿using NeuralJourney.Library.Constants;
using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Options;
using Serilog;
using System.Net;
using System.Net.Sockets;

namespace NeuralJourney.Logic.Handlers
{
    public class ConnectionHandler : IConnectionHandler
    {
        private readonly CancellationTokenSource _cts;
        private readonly TcpListener _tcpListener;

        private readonly ILogger _logger;

        public event Action<Player>? OnPlayerConnected;

        public ConnectionHandler(ServerOptions serverOptions, ILogger logger)
        {
            _cts = new CancellationTokenSource();
            _tcpListener = new TcpListener(IPAddress.Any, serverOptions.Port);
            _logger = logger;
        }

        public async Task HandleConnectionsAsync()
        {
            _tcpListener.Start();

            try
            {
                _logger.Information(InfoMessageTemplates.ServerStarted);

                while (!_cts.Token.IsCancellationRequested)
                {
                    var client = await _tcpListener.AcceptTcpClientAsync();

                    _logger.Information(InfoMessageTemplates.ClientConnected, client.Client.RemoteEndPoint);

                    if (client is null)
                        continue;

                    OnPlayerConnected?.Invoke(new Player(client));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
            }
            finally
            {
                _tcpListener.Stop();

                _logger.Information(InfoMessageTemplates.ServerStopped);
            }
        }

        public void Stop()
        {
            _cts.Cancel();
        }
    }
}