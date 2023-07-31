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
        private bool Initialized;

        public GameServer(IGameEngine gameEngine, ServerOptions serverOptions)
        {
            _gameEngine = gameEngine;
            _tcpListener = new TcpListener(IPAddress.Any, serverOptions.Port);
        }

        public async Task<GameServer> Init()
        {
            await _gameEngine.Init().StartAsync();

            Initialized = true;

            return this;
        }

        public async Task Run()
        {
            if (!Initialized)
                Console.WriteLine("The GameServer have not been initialized yet. A call to Init() must be made before running.");

            IsRunning = true;

            var adminTask = HandleAdminInputAsync();
            var clientTask = HandleClientsAsync();

            await Task.WhenAll(adminTask, clientTask);
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

                    if (adminInput == "exit")
                    {
                        IsRunning = false;
                        continue;
                    }

                    _ = _gameEngine.HandleAdminCommandAsync(adminInput);
                }
            });
        }

        private Task HandleClientsAsync()
        {
            return Task.Run(async () =>
            {
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
                    _tcpListener.Stop();
                }
            });
        }
    }
}