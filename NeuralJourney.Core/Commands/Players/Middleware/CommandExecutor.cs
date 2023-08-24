using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Models.LogProperties;

namespace NeuralJourney.Core.Commands.Players.Middleware
{
    public class CommandExecutor : ICommandMiddleware
    {
        public async Task InvokeAsync(CommandContext context, Func<Task> next, CancellationToken cancellationToken = default)
        {
            if (context.Command is null)
                throw new InvalidOperationException("Could not execute command");

            context.Result = await context.Command.ExecuteAsync();

            await next();
        }
    }
}