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
        private readonly int _serverPort;

        private CancellationTokenSource? _cts;

        private const string _inputPrefix = "> ";

        public GameClient(IMessageService messageService, ClientOptions options)
        {
            _tcpClient = new TcpClient();
            _serverIp = options.ServerIp;
            _serverPort = options.ServerPort;
            _messageService = messageService;
        }

        public async Task<GameClient> Init(CancellationTokenSource cts)
        {
            _cts = cts;

            _ = Task.Run(() =>
            {
                Console.WriteLine("Press ESC to exit\n\nConnecting to the server...");
                while (!_cts.IsCancellationRequested)
                {
                    if (Console.ReadKey(intercept: true).Key == ConsoleKey.Escape)
                    {
                        Console.WriteLine("Quitting the game. Goodbye!");
                        Stop();
                    }
                }
            });

            var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(10));

            try
            {
                await _tcpClient.ConnectAsync(_serverIp, _serverPort, timeoutCts.Token);
            }
            catch (OperationCanceledException ex)
            {
                if (ex.CancellationToken == timeoutCts.Token)
                    Console.WriteLine("Could not connect to the server. Quitting...");
                else
                    Console.WriteLine("Quitting the game. Goodbye!");
            }

            return this;
        }

        public async Task Run()
        {
            if (_cts is null)
                throw new InvalidOperationException("Failed to start the game. Reason: The client have not been initialized.");

            try
            {
                var stream = _tcpClient.GetStream();

                var clientInputTask = HandleClientInputAsync(stream, _cts.Token);
                var serverMessageTask = HandleServerMessagesAsync(stream, _cts.Token);

                await Task.WhenAny(clientInputTask, serverMessageTask);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("The game stopped unexpectedly.");
                Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Stop()
        {
            _tcpClient.Close();

            if (_cts is null)
                return;

            _cts.Cancel();
            _cts.Dispose();
        }

        private async Task HandleClientInputAsync(NetworkStream stream, CancellationToken cancellationToken)
        {
            Console.Write(_inputPrefix);
            while (!cancellationToken.IsCancellationRequested)
            {
                var input = await Task.Run(Console.In.ReadLineAsync, cancellationToken);
                if (string.IsNullOrEmpty(input))
                    continue;

                await _messageService.SendMessageAsync(stream, input, cancellationToken);
            }
        }

        private async Task HandleServerMessagesAsync(NetworkStream stream, CancellationToken cancellationToken)
        {
            string? message = default;
            while (!cancellationToken.IsCancellationRequested)
            {
                while (message is null)
                {
                    if (!stream.DataAvailable)
                        continue;

                    message = await _messageService.ReadMessageAsync(stream, cancellationToken);
                    if (_messageService.IsCloseConnectionMessage(message))
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        Console.WriteLine("Server closed the connection unexpectedly.");

                        return;
                    }
                }

                if (string.IsNullOrEmpty(message))
                    continue;

                Console.WriteLine(message);
                Console.Write(_inputPrefix);
                message = default;
            }
        }
    }
}