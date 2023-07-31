using CrawlerGame.Logic.Options;
using System.Net.Sockets;
using System.Text;

namespace CrawlerGame.Client
{
    internal class GameClient
    {
        private readonly TcpClient _tcpClient;
        private readonly string _serverIp;
        private readonly int _port;

        private const string InputPrefix = "> ";

        private Task<string?>? PlayerInputTask;
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

            try
            {
                using var stream = _tcpClient.GetStream();
                while (IsRunning)
                {
                    PlayerInputTask ??= Task.Run(Console.In.ReadLineAsync);

                    if (!PlayerInputTask.IsCompleted)
                        continue;

                    var playerInput = await PlayerInputTask;
                    PlayerInputTask = default;

                    if (string.IsNullOrEmpty(playerInput))
                        continue;

                    var data = Encoding.UTF8.GetBytes(playerInput);
                    await stream.WriteAsync(data);

                    string? response = default;
                    while (response is null)
                    {
                        if (!stream.DataAvailable)
                            continue;

                        var buffer = new byte[4096];
                        var bytesRead = await stream.ReadAsync(buffer);
                        response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    }

                    Console.WriteLine(response);
                    Console.Write(InputPrefix);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                _tcpClient.Close();
            }
        }
    }
}