using NeuralJourney.Library.Constants;
using NeuralJourney.Logic.Engines.Interfaces;
using NeuralJourney.Logic.Handlers.Interfaces;
using NeuralJourney.Logic.Options;
using NeuralJourney.Logic.Services.Interfaces;
using Serilog;
using System.Net.Sockets;

namespace NeuralJourney.Logic.Engines
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

        public ClientEngine(IInputHandler<TextReader> consoleInputHandler, IInputHandler<NetworkStream> networkInputHandler, IMessageService messageService, ILogger logger, ClientOptions options)
        {
            _consoleInputHandler = consoleInputHandler;
            _networkInputHandler = networkInputHandler;
            _messageService = messageService;

            _logger = logger;

            _client = new TcpClient();

            _serverIp = options.ServerIp;
            _serverPort = options.ServerPort;

            _networkInputHandler.OnInputReceived += _networkInputHandler_OnInputReceived;
            _consoleInputHandler.OnInputReceived += _consoleInputHandler_OnInputReceived;
        }

        public async Task<IEngine> Init(CancellationToken cancellationToken)
        {
            _logger.Information(ClientMessageTemplates.ConnectionInitialize);

            var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            timeoutCts.CancelAfter(TimeSpan.FromSeconds(10));

            try
            {
                await _client.ConnectAsync(_serverIp, _serverPort, cancellationToken);
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

        public async Task Run(CancellationToken cancellationToken)
        {
            var stream = _client.GetStream();

            var serverMessageTask = _networkInputHandler.HandleInputAsync(stream, cancellationToken);
            var clientInputTask = _consoleInputHandler.HandleInputAsync(Console.In, cancellationToken);

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
            await _messageService.SendCloseConnectionAsync(_client.GetStream());

            _client.Close();
        }

        private void _consoleInputHandler_OnInputReceived(string input, TextReader console)
        {
            _messageService.SendMessageAsync(_client.GetStream(), input);
        }

        private void _networkInputHandler_OnInputReceived(string message, NetworkStream sender)
        {
            _logger.Information(message);
        }
    }
}