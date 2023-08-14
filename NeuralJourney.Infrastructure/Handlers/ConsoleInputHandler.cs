using NeuralJourney.Core.Interfaces.Handlers;
using NeuralJourney.Core.Interfaces.Services;
using Serilog;

namespace NeuralJourney.Infrastructure.Handlers
{
    public class ConsoleInputHandler : IInputHandler<TextReader>
    {
        private readonly IMessageService _messageService;
        private readonly ILogger _logger;


        public ConsoleInputHandler(IMessageService messageService, ILogger logger)
        {
            _messageService = messageService;
            _logger = logger.ForContext<ConsoleInputHandler>();
        }

        public event Action<string, TextReader>? OnInputReceived;
        public event Action<TextReader>? OnClosedConnection;

        public async Task HandleInputAsync(TextReader reader, CancellationToken cancellationToken)
        {
            Task<string?>? readTask = null;
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    readTask ??= Task.Run(() => Console.ReadLine());

                    await Task.WhenAny(readTask, Task.Delay(-1, cancellationToken));

                    cancellationToken.ThrowIfCancellationRequested();

                    var input = await readTask;
                    readTask = null;

                    if (_messageService.IsCloseConnectionMessage(input ?? string.Empty))
                    {
                        OnClosedConnection?.Invoke(reader);
                        return;
                    }

                    if (string.IsNullOrEmpty(input))
                        continue;

                    OnInputReceived?.Invoke(input, reader);
                }
                catch (OperationCanceledException)
                {
                    return; // Return back to caller who initialized cancellation
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, ex.Message);
                }
            }
        }
    }
}