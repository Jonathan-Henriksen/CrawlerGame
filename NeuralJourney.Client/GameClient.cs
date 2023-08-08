using NeuralJourney.Logic.Options;
using NeuralJourney.Logic.Services;
using System.Net.Sockets;

namespace NeuralJourney.Client
{
    internal class GameClient
    {
        private readonly IMessageService _messageService;

        private readonly TcpClient _tcpClient;
        private readonly string _serverIp;
        private readonly int _port;

        private const string InputPrefix = "> ";

        private bool IsRunning;

        public GameClient(IMessageService messageService, ClientOptions options)
        {
            _tcpClient = new TcpClient();
            _serverIp = options.ServerIp;
            _port = options.ServerPort;
            _messageService = messageService;
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

            using var stream = _tcpClient.GetStream();

            var clientInputTask = HandleClientInputAsync(stream);
            var serverMessageTask = HandleServerMessagesAsync(stream);

            await Task.WhenAll(clientInputTask, serverMessageTask);
        }

        private async Task HandleClientInputAsync(NetworkStream stream)
        {
            try
            {
                Console.Write(InputPrefix);
                while (IsRunning)
                {
                    var input = await Task.Run(Console.In.ReadLineAsync);
                    if (string.IsNullOrEmpty(input))
                        continue;

                    await _messageService.SendMessageAsync(stream, input);
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

                        message = await _messageService.ReadMessageAsync(stream);
                        if (_messageService.IsCloseConnectionMessage(message))
                        {
                            IsRunning = false;
                            return;
                        }
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