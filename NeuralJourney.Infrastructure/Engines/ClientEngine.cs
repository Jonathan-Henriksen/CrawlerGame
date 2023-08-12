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
        private readonly IInputHandler<NetworkStream> _networkInputHandler;

        private readonly IMessageService _messageService;
        private readonly ILogger _logger;

        private readonly TcpClient _client;
        private readonly string _serverIp;
        private readonly int _serverPort;

        private CancellationTokenSource _tokenSource;

        public ClientEngine(IInputHandler<TextReader> consoleInputHandler, IInputHandler<NetworkStream> networkInputHandler, IMessageService messageService, ILogger logger, ClientOptions options)
        {
            _consoleInputHandler = consoleInputHandler;
            _networkInputHandler = networkInputHandler;

            _messageService = messageService;
            _logger = logger;

            _tokenSource = new CancellationTokenSource();
            _client = new TcpClient();

            _serverIp = options.ServerIp;
            _serverPort = options.ServerPort;

            _networkInputHandler.OnInputReceived += HandleNetworkInputReceived;
            _networkInputHandler.OnClosedConnection += HandleClosedConnection;

            _consoleInputHandler.OnInputReceived += HandleConsoleInputReceived;
        }

        public async Task Run(CancellationToken cancellationToken = default)
        {
            if (cancellationToken != default)
                _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _logger.Information(ClientMessageTemplates.ConnectionInitialize);

            var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(_tokenSource.Token);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(10));

            try
            {
                await _client.ConnectAsync(_serverIp, _serverPort, timeoutCts.Token);
                _logger.Information(ClientMessageTemplates.ConnectionEstablished);
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
            {
                _logger.Information(ClientMessageTemplates.ConnectionFailed);
                throw; // Throw back to main
            }
            catch (OperationCanceledException)
            {
                throw; // Throw back to main
            }
            catch (SocketException socketException)
            {
                _logger.Error(socketException, socketException.Message);
                _logger.Information(ClientMessageTemplates.ConnectionFailed);
            }

            await StartInputHandlersAsync();
        }

        public async Task Stop()
        {
            if (!_client.Connected)
                return;

            await _messageService.SendCloseConnectionAsync(_client.GetStream(), _tokenSource.Token);

            _tokenSource.Cancel();

            _client.Close();
        }

        private async Task StartInputHandlersAsync()
        {
            var networkInputTask = _networkInputHandler.HandleInputAsync(_client.GetStream(), _tokenSource.Token);
            var consoleInputTask = _consoleInputHandler.HandleInputAsync(Console.In, _tokenSource.Token);

            try
            {
                await Task.WhenAny(consoleInputTask, networkInputTask);
            }
            catch (OperationCanceledException)
            {
                throw; // Throw back to main
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An unexpected error occured. Message: {ErrorMessage}", ex.Message);
            }
        }

        private async void HandleConsoleInputReceived(string input, TextReader reader) => await _messageService.SendMessageAsync(_client.GetStream(), input, _tokenSource.Token);

        private void HandleNetworkInputReceived(string message, NetworkStream sender) => _logger.Information(message);

        private void HandleClosedConnection(NetworkStream sender)
        {
            _logger.Information("The server closed the connection");
            _tokenSource.Cancel();
        }

        public void Dispose()
        {
            _networkInputHandler.OnInputReceived -= HandleNetworkInputReceived;
            _networkInputHandler.OnClosedConnection -= HandleClosedConnection;

            _consoleInputHandler.OnInputReceived -= HandleConsoleInputReceived;

            if (!_tokenSource.IsCancellationRequested)
                _tokenSource.Cancel();

            _tokenSource.Dispose();

            _client.Close();
            _client.Dispose();

            _logger.Debug(DebugMessageTemplates.DispoedOfType, GetType().Name);
        }
    }
}
