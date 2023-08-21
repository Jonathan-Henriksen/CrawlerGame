using NeuralJourney.Core.Models.LogProperties;

namespace NeuralJourney.Core.Interfaces.Commands
{
    public interface ICommandStrategy
    {
        Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken = default);
    }
}