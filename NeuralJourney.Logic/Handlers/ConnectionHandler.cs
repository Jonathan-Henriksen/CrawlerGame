using NeuralJourney.Library.Constants;
using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Options;
using NeuralJourney.Logic.Services;
using System.Net;
using System.Net.Sockets;

namespace NeuralJourney.Logic.Handlers
{
    public class ConnectionHandler : IConnectionHandler
    {
        private readonly IMessageService _messageService;
        private readonly TcpListener _tcpListener;

        public event Action<Player>? OnPlayerConnected;

        private bool IsRunning;

        public ConnectionHandler(IMessageService messageService, ServerOptions serverOptions)
        {
            _messageService = messageService;
            _tcpListener = new TcpListener(IPAddress.Any, serverOptions.Port);

            IsRunning = false;
        }

        public async Task HandleConnectionsAsync()
        {
            IsRunning = true;
            _tcpListener.Start();

            try
            {
                Console.WriteLine(InfoMessages.System.StartingServer);

                while (IsRunning)
                {
                    var client = await _tcpListener.AcceptTcpClientAsync();
                    Console.WriteLine($"{InfoMessages.System.ClientConnected}: {client.Client.RemoteEndPoint}");

                    if (client is null)
                        continue;

                    var stream = client.GetStream();
                    await _messageService.SendMessageAsync(stream, "Welcome to NeuralJourney!");

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
            IsRunning = false;
        }
    }
}