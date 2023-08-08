using NeuralJourney.Library.Constants;
using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Options;
using System.Net;
using System.Net.Sockets;

namespace NeuralJourney.Logic.Handlers
{
    public class ConnectionHandler : IConnectionHandler
    {
        private readonly CancellationTokenSource _cts;
        private readonly TcpListener _tcpListener;


        public event Action<Player>? OnPlayerConnected;

        public ConnectionHandler(ServerOptions serverOptions)
        {
            _cts = new CancellationTokenSource();
            _tcpListener = new TcpListener(IPAddress.Any, serverOptions.Port);
        }

        public async Task HandleConnectionsAsync()
        {
            _tcpListener.Start();

            try
            {
                Console.WriteLine(InfoMessages.System.StartingServer);

                while (!_cts.Token.IsCancellationRequested)
                {
                    var client = await _tcpListener.AcceptTcpClientAsync();
                    Console.WriteLine($"{InfoMessages.System.ClientConnected}: {client.Client.RemoteEndPoint}");

                    if (client is null)
                        continue;

                    OnPlayerConnected?.Invoke(new Player(client));
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
            finally
            {
                _tcpListener.Stop();
            }
        }

        public void Stop()
        {
            _cts.Cancel();
        }
    }
}