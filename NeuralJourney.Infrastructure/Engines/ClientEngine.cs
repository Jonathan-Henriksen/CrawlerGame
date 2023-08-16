using NeuralJourney.Core.Constants.Messages;
using NeuralJourney.Core.Interfaces.Engines;
using NeuralJourney.Core.Interfaces.Handlers;
using NeuralJourney.Core.Interfaces.Services;
using NeuralJourney.Core.Options;
using Serilog;
using System.Net.Sockets;

namespace NeuralJourney.Infrastructure.Engines
{
    public class ClientEngine : IEngine
    {
        private readonly IInputHandler<TextReader> _consoleInputHandler;
        private readonly IInputHandler<TcpClient> _networkInputHandler;

        private readonly IMessageService _messageService;
        private readonly ILogger _logger;

        private readonly TcpClient _client;
        private readonly string _serverIp;
        private readonly int _serverPort;

        private CancellationToken _token;

        public ClientEngine(IInputHandler<TextReader> consoleInputHandler, IInputHandler<TcpClient> networkInputHandler, IMessageService messageService, ILogger logger, ClientOptions options)
        {
            _consoleInputHandler = consoleInputHandler;
            _networkInputHandler = networkInputHandler;

            _messageService = messageService;
            _logger = logger.ForContext<ClientEngine>();

            _client = new TcpClient();

            _serverIp = options.ServerIp;
            _serverPort = options.ServerPort;

            _networkInputHandler.OnInputReceived += HandleNetworkInputReceived;
            _networkInputHandler.OnClosedConnection += HandleClosedConnection;

            _consoleInputHandler.OnInputReceived += HandleConsoleInputReceived;
        }

        public async Task Run(CancellationToken cancellationToken = default)
        {
            _logger.Information(ClientMessageTemplates.StartingGame);

            _token = cancellationToken;

            _logger.Information(ClientMessageTemplates.ConnectionInitialize);

            var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(_token);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(10));

            try
            {
                await _client.ConnectAsync(_serverIp, _serverPort, timeoutCts.Token);
                _logger.Information(ClientMessageTemplates.ConnectionEstablished);
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
            {
                if (timeoutCts.IsCancellationRequested)
                    _logger.Information(ClientMessageTemplates.ConnectionFailed);

                await Stop();
                return;
            }
            catch (SocketException ex)
            {
                _logger.Error(ex, ex.Message);
                _logger.Information(ClientMessageTemplates.ConnectionFailed);

                await Stop();

                return;
            }

            await StartInputHandlersAsync();
        }

        public async Task Stop()
        {
            _logger.Information(ClientMessageTemplates.StoppingGame);

            if (!_client.Connected)
                return;

            await _messageService.SendCloseConnectionAsync(_client, _token);

            _client.Close();
        }

        private async Task StartInputHandlersAsync()
        {
            var networkInputTask = _networkInputHandler.HandleInputAsync(_client, _token);
            var consoleInputTask = _consoleInputHandler.HandleInputAsync(Console.In, _token);

            try
            {
                await Task.WhenAny(consoleInputTask, networkInputTask);
            }
            catch (OperationCanceledException)
            {
                // Stopped from cancellation event
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An unexpected error occured");
                await Stop();
            }
        }

        private async void HandleConsoleInputReceived(string input, TextReader reader) => await _messageService.SendMessageAsync(_client, input, _token);

        private void HandleNetworkInputReceived(string message, TcpClient sender) => _logger.Information(message);

        private void HandleClosedConnection(TcpClient sender)
        {
            _logger.Information(ClientMessageTemplates.ConnectionClosed);
            _ = Stop();
        }

        public void Dispose()
        {
            _networkInputHandler.OnInputReceived -= HandleNetworkInputReceived;
            _networkInputHandler.OnClosedConnection -= HandleClosedConnection;

            _consoleInputHandler.OnInputReceived -= HandleConsoleInputReceived;

            _client.Close();
            _client.Dispose();

            _logger.Debug(DebugMessageTemplates.DispoedOfType, GetType().Name);
        }
    }
}
