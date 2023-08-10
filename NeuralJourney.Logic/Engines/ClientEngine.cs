using NeuralJourney.Library.Constants;
using NeuralJourney.Logic.Options;
using NeuralJourney.Logic.Services;
using Serilog;
using System.Net.Sockets;

namespace NeuralJourney.Logic.Engines
{
    public class ClientEngine : IEngine

    {
        private readonly IMessageService _messageService;
        private readonly ILogger _logger;
        private readonly TcpClient _tcpClient;

        private readonly string _serverIp;
        private readonly int _serverPort;

        private CancellationTokenSource? _cts;

        private const string _inputPrefix = "> ";

        public ClientEngine(IMessageService messageService, ILogger logger, ClientOptions options)
        {
            _messageService = messageService;
            _logger = logger;
            _tcpClient = new TcpClient();
            _serverIp = options.ServerIp;
            _serverPort = options.ServerPort;
        }

        public async Task<IEngine> Init(CancellationTokenSource cts)
        {
            _cts = cts;

            _ = Task.Run(async () =>
            {
                _logger.Information("Press ESC to exit\n");
                _logger.Information(ClientMessageTemplates.ConnectionInitialize);
                while (!_cts.IsCancellationRequested)
                {
                    if (Console.ReadKey(intercept: true).Key == ConsoleKey.Escape)
                    {
                        _logger.Information(ClientMessageTemplates.StopGame);

                        await _messageService.SendCloseConnectionAsync(_tcpClient.GetStream(), _cts.Token);

                        _ = Stop();
                        continue;
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
                    _logger.Information(ClientMessageTemplates.ConnectionFailed);
                else
                    _logger.Information(ClientMessageTemplates.StopGame);

                throw;
            }
            catch (SocketException)
            {
                _logger.Information(ClientMessageTemplates.ConnectionFailed);
            }

            _logger.Information(ClientMessageTemplates.ConnectionEstablished);

            return this;
        }

        public async Task Run()
        {
            if (_cts is null)
                throw new InvalidOperationException(ClientMessageTemplates.ClientNotInitialized);

            var stream = _tcpClient.GetStream();

            var serverMessageTask = HandleServerMessagesAsync(stream, _cts.Token);
            var clientInputTask = HandleClientInputAsync(stream, _cts.Token);

            try
            {
                await Task.WhenAny(clientInputTask, serverMessageTask);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _ = Stop();
            }
        }

        public async Task Stop()
        {
            if (_cts is null)
                throw new InvalidOperationException(ClientMessageTemplates.ClientNotInitialized);

            await _messageService.SendCloseConnectionAsync(_tcpClient.GetStream(), _cts.Token);

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

                var input = await Console.In.ReadLineAsync(cancellationToken);

                if (string.IsNullOrEmpty(input))
                    continue;

                _ = _messageService.SendMessageAsync(stream, input, cancellationToken);

                Console.Write(_inputPrefix);
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
                    {
                        await Task.Delay(100, cancellationToken);
                        continue;
                    }

                    message = await _messageService.ReadMessageAsync(stream, cancellationToken);
                    if (_messageService.IsCloseConnectionMessage(message))
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        _logger.Information(ClientMessageTemplates.ConnectionClosed);

                        _ = Stop();
                        return;
                    }
                }

                if (string.IsNullOrEmpty(message))
                    continue;

                _logger.Information(message);

                message = default;
            }
        }
    }
}