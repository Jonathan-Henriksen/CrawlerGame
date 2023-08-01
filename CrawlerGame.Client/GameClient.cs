using CrawlerGame.Library.Extensions;
using CrawlerGame.Logic.Options;
using System.Net.Sockets;

namespace CrawlerGame.Client
{
    internal class GameClient
    {
        private readonly TcpClient _tcpClient;
        private readonly string _serverIp;
        private readonly int _port;

        private const string InputPrefix = "> ";

        private bool IsRunning;

        public GameClient(ClientOptions options)
        {
            _tcpClient = new TcpClient();
            _serverIp = options.ServerIp;
            _port = options.ServerPort;
        }

        public async Task<GameClient> Init()
        {
            await _tcpClient.ConnectAsync(_serverIp, _port);

            Console.WriteLine("Connected to the server.");

            return this;
        }

        public async Task Run()
        {
            IsRunning = true;
            Console.Write(InputPrefix);

            using var stream = _tcpClient.GetStream();

            var clientInputTask = HandleClientInputAsync(stream);
            var serverMessageTask = HandleServerMessagesAsync(stream);


            await Task.WhenAll(clientInputTask, serverMessageTask);
        }

        private async Task HandleClientInputAsync(NetworkStream stream)
        {
            try
            {
                while (IsRunning)
                {
                    var input = await Task.Run(Console.In.ReadLineAsync);

                    if (string.IsNullOrEmpty(input))
                        continue;

                    await stream.SendMessageAsync(input);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task HandleServerMessagesAsync(NetworkStream stream)
        {
            try
            {
                string? message = default;
                while (IsRunning)
                {
                    while (message is null)
                    {
                        if (!stream.DataAvailable)
                            continue;

                        message = await stream.ReadMessageAsync();
                    }

                    if (string.IsNullOrEmpty(message))
                        continue;

                    Console.WriteLine(message);
                    Console.Write(InputPrefix);

                    message = default;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}