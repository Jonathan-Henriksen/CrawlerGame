using CrawlerGame.Logic.Options;
using System.Net.Sockets;

namespace CrawlerGame.Client
{
    internal class PlayerClient : IClient
    {
        private readonly TcpClient _tcpClient;
        private readonly string _serverIp;
        private readonly int _port;

        private Task<string?>? PlayerInputTask;
        private bool IsRunning;

        public PlayerClient(ClientOptions options)
        {
            _tcpClient = new TcpClient();
            _serverIp = options.ServerIp;
            _port = options.ServerPort;
        }

        public async Task Start()
        {
            IsRunning = true;

            try
            {
                await _tcpClient.ConnectAsync(_serverIp, _port);
                Console.WriteLine("Connected to the server.");

                while (IsRunning)
                {
                    PlayerInputTask ??= Task.Run(Console.In.ReadLineAsync);

                    if (Console.KeyAvailable && Console.ReadKey().Key == ConsoleKey.Escape)
                    {
                        IsRunning = false;
                        continue;
                    }

                    if (!PlayerInputTask.IsCompleted)
                        continue;

                    var playerInput = await PlayerInputTask;
                    PlayerInputTask = default;

                    if (string.IsNullOrEmpty(playerInput))
                        continue;

                    var stream = _tcpClient.GetStream();

                    using var writer = new StreamWriter(stream) { AutoFlush = true };
                    await writer.WriteAsync(playerInput);


                    using var reader = new StreamReader(stream);
                    var response = await reader.ReadLineAsync();

                    Console.WriteLine(response);


                    _ = stream.DisposeAsync();
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

    public interface IClient
    {
        Task Start();
    }
}
