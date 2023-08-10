using NeuralJourney.Library.Models.Commands;

namespace NeuralJourney.Logic.Commands.Middleware
{
    public interface ICommandMiddleware
    {
        Task InvokeAsync(CommandContext context, Func<Task> next, CancellationToken cancellationToken = default);
    }
}
