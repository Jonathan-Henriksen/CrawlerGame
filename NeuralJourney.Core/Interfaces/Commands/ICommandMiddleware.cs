using NeuralJourney.Core.Models.Commands;

namespace NeuralJourney.Core.Interfaces.Commands
{
    public interface ICommandMiddleware
    {
        Task InvokeAsync(CommandContext context, Func<Task> next, CancellationToken cancellationToken = default);
    }
}
