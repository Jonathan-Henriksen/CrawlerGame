using CrawlerGame.Logic;
using CrawlerGame.Logic.Options;
using System.Net;
using System.Net.Sockets;

namespace CrawlerGame.Server
{
    internal class GameServer
    {
        private readonly IGameEngine _gameEngine;
        private readonly TcpListener _tcpListener;

        private bool IsRunning;

        public GameServer(IGameEngine gameEngine, ServerOptions serverOptions)
        {
            _gameEngine = gameEngine;
            _tcpListener = new TcpListener(IPAddress.Any, serverOptions.Port);
        }

        public GameServer Init()
        {
            IsRunning = true;

            _ = _gameEngine.Init().StartAsync();
            _ = HandleAdminInputAsync();

            return this;
        }

        public async Task Start()
        {
            if (!IsRunning)
                Console.WriteLine("The GameServer have not been initialized yet. A call to Init() must be made before starting.");

            _tcpListener.Start();

            try
            {
                Console.WriteLine("Starting server and waiting for clients");
                while (IsRunning)
                {
                    var client = await _tcpListener.AcceptTcpClientAsync();

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
                _gameEngine.Stop();
                _tcpListener.Stop();
            }
        }

        private Task HandleAdminInputAsync()
        {
            return Task.Run(async () =>
            {
                Task<string?>? adminInputTask = default;
                while (IsRunning)
                {
                    adminInputTask ??= Task.Run(Console.In.ReadLineAsync);

                    if (!adminInputTask.IsCompleted)
                        continue;

                    var adminInput = await adminInputTask;
                    adminInputTask = default;

                    if (string.IsNullOrEmpty(adminInput))
                        continue;

                    IsRunning = !adminInput.Equals("exit", StringComparison.InvariantCultureIgnoreCase);
                }
            });
        }
    }
}