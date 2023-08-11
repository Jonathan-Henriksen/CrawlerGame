using NeuralJourney.Core.Interfaces.Commands;

namespace NeuralJourney.Core.Commands.Middleware
{
    public class CommandExecutor : ICommandMiddleware
    {
        public async Task InvokeAsync(CommandContext context, Func<Task> next, CancellationToken cancellationToken = default)
        {
            if (context.Command is null)
                throw new InvalidOperationException("Failed to execute command. Reason: Command was null at point of execution.");

            context.Result = await context.Command.ExecuteAsync();

            await next();
        }
    }
}