using NeuralJourney.Core.Models.LogProperties;

namespace NeuralJourney.Core.Interfaces.Commands
{
    public interface ICommandMiddleware
    {
        Task InvokeAsync(CommandContext context, Func<Task> next, CancellationToken cancellationToken = default);
    }
}
