using CrawlerGame.Library.Models.Player;
using CrawlerGame.Logic.Options;
using System.Net.Sockets;
using System.Numerics;
using System.Text;

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

                using var stream = _tcpClient.GetStream();
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

                    var data = Encoding.UTF8.GetBytes(playerInput);
                    await stream.WriteAsync(data, 0, data.Length);

                    string? response = default;
                    while (response is null)
                    {
                        if (!stream.DataAvailable)
                            continue;

                        var buffer = new byte[4096];
                        var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    }

                    Console.WriteLine(response);
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
