using NeuralJourney.Core.Constants;
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

        private CancellationTokenSource _cts;

        public ClientEngine(IInputHandler<TextReader> consoleInputHandler, IInputHandler<NetworkStream> networkInputHandler, IMessageService messageService, ILogger logger, ClientOptions options)
        {
            _consoleInputHandler = consoleInputHandler;
            _networkInputHandler = networkInputHandler;

            _messageService = messageService;
            _logger = logger;

            _cts = new CancellationTokenSource();
            _client = new TcpClient();

            _serverIp = options.ServerIp;
            _serverPort = options.ServerPort;

            _networkInputHandler.OnInputReceived += HandleNetworkInputReceived;
            _consoleInputHandler.OnInputReceived += HandleConsoleInputReceived;
        }

        public async Task Run(CancellationToken cancellationToken = default)
        {
            if (cancellationToken != default)
                _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _logger.Information(ClientMessageTemplates.ConnectionInitialize);

            var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(10));

            try
            {
                await _client.ConnectAsync(_serverIp, _serverPort, timeoutCts.Token);
                _logger.Information(ClientMessageTemplates.ConnectionEstablished);
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
            {
                _logger.Information(ClientMessageTemplates.ConnectionFailed);
                throw;
            }
            catch (SocketException socketException)
            {
                _logger.Error(socketException, socketException.Message);
                _logger.Information(ClientMessageTemplates.ConnectionFailed);
                throw;
            }

            await StartInputHandlersAsync();
        }

        public async Task Stop()
        {
            if (!_client.Connected)
                return;

            await _messageService.SendCloseConnectionAsync(_client.GetStream(), _cts.Token);
            _client.Close();
        }

        private async Task StartInputHandlersAsync()
        {
            var networkInputTask = _networkInputHandler.HandleInputAsync(_client.GetStream(), _cts.Token);
            var consoleInputTask = _consoleInputHandler.HandleInputAsync(Console.In, _cts.Token);

            try
            {
                await Task.WhenAll(consoleInputTask, networkInputTask);
            }
            catch (OperationCanceledException)
            {
                _logger.Information(ClientMessageTemplates.ConnectionFailed);
                throw;
            }
        }

        private async void HandleConsoleInputReceived(string input, TextReader console) => await _messageService.SendMessageAsync(_client.GetStream(), input, _cts.Token);

        private void HandleNetworkInputReceived(string message, NetworkStream sender) => _logger.Information(message);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _networkInputHandler.OnInputReceived -= HandleNetworkInputReceived;
                _consoleInputHandler.OnInputReceived -= HandleConsoleInputReceived;

                _cts.Cancel();
                _cts.Dispose();

                _client.Dispose();
            }
        }
    }
}
