using NeuralJourney.Library.Models.Commands;

namespace NeuralJourney.Logic.Commands
{
    public interface ICommandStrategy
    {
        Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken = default);
    }
}