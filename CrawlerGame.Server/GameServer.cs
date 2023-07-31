using CrawlerGame.Logic;
using CrawlerGame.Logic.Options;
using System.Net;
using System.Net.Sockets;

namespace CrawlerGame.Server
{
    internal class GameServer
    {
        private readonly IGameEngine _gameEngine;
        private readonly TcpListener _server;

        private bool IsRunning;

        public GameServer(IGameEngine gameEngine, ServerOptions serverOptions)
        {
            _gameEngine = gameEngine;
            _server = new TcpListener(IPAddress.Any, serverOptions.Port);
        }

        public GameServer Init()
        {
            _gameEngine.Init().Start();
            _server.Start();

            IsRunning = true;

            return this;
        }

        public async Task Start()
        {
            if (!IsRunning)
                Console.WriteLine("The GameServer have not been initialized yet. A call to Init() must be made before starting.");

            try
            {
                Console.WriteLine("Starting server and waiting for clients");
                while (IsRunning)
                {
                    if (Console.KeyAvailable && Console.ReadKey().Key == ConsoleKey.Escape)
                        break;

                    var client = await _server.AcceptTcpClientAsync();

                    Console.WriteLine($"Client connected: {client.Client.RemoteEndPoint}");

                    _gameEngine.AddPlayer(client);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
            finally
            {
                Stop();
            }
        }

        public void Stop()
        {
            IsRunning = false;

            _gameEngine.Stop();
            _server.Stop();
        }
    }
}