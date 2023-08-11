using NeuralJourney.Core.Interfaces.Handlers;
using Serilog;

namespace NeuralJourney.Infrastructure.Handlers
{
    public class ConsoleInputHandler : IInputHandler<TextReader>
    {
        private readonly ILogger _logger;

        private CancellationTokenSource _tokenSource;

        public ConsoleInputHandler(ILogger logger)
        {
            _logger = logger;
            _tokenSource = new CancellationTokenSource();
        }

        public event Action<string, TextReader>? OnInputReceived;
        public event Action<TextReader>? OnClosedConnection;

        public async Task HandleInputAsync(TextReader reader, CancellationToken cancellationToken = default)
        {
            if (cancellationToken != default)
                _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

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
                OnClosedConnection?.Invoke(reader);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected error in ConsoleInputHandler: {ErrorMessage}", ex.Message);
            }
        }


        public void Dispose()
        {
            _logger.Debug("Disposing of {Type}", GetType().Name);

            _tokenSource.Cancel();
            _tokenSource.Dispose();
        }
    }
}