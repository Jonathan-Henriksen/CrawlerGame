using NeuralJourney.Core.Interfaces.Handlers;
using Serilog;

namespace NeuralJourney.Infrastructure.Handlers
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
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            try
            {
                while (!cts.IsCancellationRequested)
                {
                    var input = await reader.ReadLineAsync(cts.Token);

                    if (string.IsNullOrEmpty(input))
                        continue;

                    OnInputReceived?.Invoke(input, reader);
                }
            }
            catch (IOException ex)
            {
                _logger.Error("Encountered an error reading console input stream: {ErrorMessage}", ex.Message);
                Cleanup(reader, cts);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected error in ConsoleInputHandler: {ErrorMessage}", ex.Message);
                Cleanup(reader, cts);
            }
        }

        private void Cleanup(TextReader reader, CancellationTokenSource cts)
        {
            OnClosedConnection?.Invoke(reader);
            cts.Cancel();
            cts.Dispose();
        }
    }
}