using NeuralJourney.Logic.Handlers.Interfaces;
using Serilog;

namespace NeuralJourney.Logic.Handlers
{
    public class ConsoleInputHandler : IInputHandler<TextReader>
    {
        private readonly ILogger _logger;

        public ConsoleInputHandler(ILogger logger)
        {
            _logger = logger;
        }

        public event Action<string, TextReader>? OnInputReceived;
        public event Action<TextReader>? OnClosedConnection;

        public async Task HandleInputAsync(TextReader reader, CancellationToken cancellationToken = default)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var input = await reader.ReadLineAsync(cancellationToken);

                    if (string.IsNullOrEmpty(input))
                        continue;

                    OnInputReceived?.Invoke(input, reader);
                }
            }
            catch (IOException ex)
            {
                _logger.Error("Console input stream encountered an error: {ErrorMessage}", ex.Message);
            }
        }
    }
}