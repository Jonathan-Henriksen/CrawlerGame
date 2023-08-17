using NeuralJourney.Core.Constants;
using NeuralJourney.Core.Extensions;
using NeuralJourney.Core.Interfaces.Engines;
using NeuralJourney.Core.Interfaces.Handlers;
using NeuralJourney.Core.Interfaces.Services;
using NeuralJourney.Core.Models.LogProperties;
using NeuralJourney.Core.Options;
using Serilog;
using Serilog.Context;
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

        private IDisposable? _loggerContext;
        private PlayerContext? _playerContext;

        public ClientEngine(IInputHandler<TextReader> consoleInputHandler, IInputHandler<TcpClient> networkInputHandler, IMessageService messageService, ILogger logger, ClientOptions options)
        {
            _consoleInputHandler = consoleInputHandler;
            _networkInputHandler = networkInputHandler;

            _messageService = messageService;

            _client = new TcpClient();

            _serverIp = options.ServerIp;
            _serverPort = options.ServerPort;

            _logger = logger.ForContext<ClientEngine>();

            _networkInputHandler.OnInputReceived += HandleNetworkInputReceived;
            _networkInputHandler.OnClosedConnection += HandleClosedConnection;

            _consoleInputHandler.OnInputReceived += HandleConsoleInputReceived;
        }

        public async Task Run(CancellationToken cancellationToken = default)
        {
            InitPlayer();

            if (_playerContext is null)
                throw new InvalidOperationException("Client have not been initialize");

            LogInfoAndDisplayInConsole(ClientLogMessages.Info.StartingGame);

            _token = cancellationToken;

            var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(_token);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(10));

            try
            {
                LogInfoAndDisplayInConsole(ClientLogMessages.Info.ConnectionInitialize);

                await _client.ConnectAsync(_serverIp, _serverPort, timeoutCts.Token);

                await ServerHandshake();

                LogInfoAndDisplayInConsole(ClientLogMessages.Info.ConnectionEstablished);
            }
            catch (OperationCanceledException ex) when (timeoutCts.IsCancellationRequested)
            {
                _messageService.DisplayConsoleMessage(ClientLogMessages.Error.ConnectionFailedTimeout);
                _logger.Error(ex, ClientLogMessages.Error.ConnectionFailedTimeout);

                await StopAsync();

                return;
            }
            catch (SocketException ex)
            {
                _messageService.DisplayConsoleMessage(ClientLogMessages.Error.ConnectionFailed);
                _logger.Error(ex, ClientLogMessages.Error.ConnectionFailed);

                await StopAsync();

                return;
            }

            await StartInputHandlersAsync();
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
                await StopAsync();

                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An unexpected error occured");
                await StopAsync();
            }
        }

        public async Task StopAsync()
        {
            LogInfoAndDisplayInConsole(ClientLogMessages.Info.StoppingGame);

            if (!_client.Connected)
                return;

            await _messageService.SendCloseConnectionAsync(_client, _token);
        }

        private void InitPlayer()
        {
            Console.Write("Please enter your name: ");
            var name = Console.ReadLine() ?? "Anonymouse";

            _playerContext = new PlayerContext(name, Guid.NewGuid(), _client.GetLocalIp());

            _loggerContext = LogContext.PushProperty("PlayerContext", _playerContext, true);
        }

        private async Task ServerHandshake()
        {
            if (_playerContext is null)
                throw new InvalidOperationException();

            await _messageService.SendHandshake(_client, _playerContext.Name, _playerContext.Id);

            var handshakeCompleted = false;
            while (!handshakeCompleted)
            {
                var response = await _messageService.ReadMessageAsync(_client, _token);
                if (!_messageService.IsHandshake(response, out var handshakeName, out var handshakeId))
                    continue;

                if (_playerContext.Name == handshakeName && _playerContext.Id == handshakeId)
                    handshakeCompleted = true;
            }
        }

        private async void HandleConsoleInputReceived(string input, TextReader reader)
        {
            await _messageService.SendMessageAsync(_client, input, _token);
        }

        private void HandleNetworkInputReceived(string message, TcpClient client) => _messageService.DisplayConsoleMessage(message);

        private void HandleClosedConnection(TcpClient client)
        {
            LogInfoAndDisplayInConsole(ClientLogMessages.Info.ConnectionClosed);

            client.Close();

            _ = StopAsync();
        }

        private void LogInfoAndDisplayInConsole(string message)
        {
            _messageService.DisplayConsoleMessage(message);
            _logger.Information(message);
        }

        public void Dispose()
        {
            _networkInputHandler.OnInputReceived -= HandleNetworkInputReceived;
            _networkInputHandler.OnClosedConnection -= HandleClosedConnection;

            _consoleInputHandler.OnInputReceived -= HandleConsoleInputReceived;

            _client.Close();
            _client.Dispose();

            _logger.Debug(SystemMessages.DispoedOfType, GetType().Name);

            _loggerContext.Dispose();
        }
    }
}