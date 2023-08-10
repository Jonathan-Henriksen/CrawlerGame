using NeuralJourney.Library.Enums.Commands;
using NeuralJourney.Library.Models.Commands;
using NeuralJourney.Library.Models.World;

namespace NeuralJourney.Logic.Handlers.Input
{
    public class ConsoleInputHandler : IInputHandler
    {
        public event Action<CommandContext>? OnInputReceived;

        public async Task HandleInputAsync(Player? player = default, CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var input = await Console.In.ReadLineAsync(cancellationToken);

                if (string.IsNullOrEmpty(input))
                    continue;

                var context = new CommandContext(input, CommandTypeEnum.Admin);

                OnInputReceived?.Invoke(context);
            }
        }
    }
}