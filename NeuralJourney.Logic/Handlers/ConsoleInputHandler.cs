using NeuralJourney.Logic.Handlers.Interfaces;

namespace NeuralJourney.Logic.Handlers
{
    public class ConsoleInputHandler : IInputHandler<TextReader>
    {
        public event Action<string, TextReader>? OnInputReceived;

        public async Task HandleInputAsync(TextReader reader, CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var input = await reader.ReadLineAsync(cancellationToken);

                if (string.IsNullOrEmpty(input))
                    continue;

                OnInputReceived?.Invoke(input, reader);
            }
        }
    }
}