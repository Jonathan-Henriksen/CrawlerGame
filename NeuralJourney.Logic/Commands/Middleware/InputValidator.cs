using NeuralJourney.Library.Enums.Commands;
using NeuralJourney.Library.Exceptions.Commands;
using NeuralJourney.Library.Models.Commands;
using NeuralJourney.Logic.Commands.Interfaces;

namespace NeuralJourney.Logic.Commands.Middleware
{
    public class InputValidation : ICommandMiddleware
    {
        public async Task InvokeAsync(CommandContext context, Func<Task> next, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(context.RawInput))
                throw new InvalidCommandException(CommandIdentifierEnum.Unknown, "Input was blank");

            await next();
        }
    }
}
