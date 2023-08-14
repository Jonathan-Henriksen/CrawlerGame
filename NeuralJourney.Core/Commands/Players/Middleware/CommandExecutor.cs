using NeuralJourney.Core.Exceptions;
using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Models.Commands;

namespace NeuralJourney.Core.Commands.Players.Middleware
{
    public class CommandExecutor : ICommandMiddleware
    {
        public async Task InvokeAsync(CommandContext context, Func<Task> next, CancellationToken cancellationToken = default)
        {
            if (context.Command is null)
                throw new CommandExecutionException("Command was null");

            context.Result = await context.Command.ExecuteAsync();

            await next();
        }
    }
}