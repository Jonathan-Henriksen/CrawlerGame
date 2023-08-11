using NeuralJourney.Core.Commands;

namespace NeuralJourney.Core.Interfaces.Commands
{
    public interface ICommandStrategy
    {
        Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken = default);
    }
}