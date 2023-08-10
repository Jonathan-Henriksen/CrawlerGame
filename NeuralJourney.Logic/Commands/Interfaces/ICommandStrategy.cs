using NeuralJourney.Library.Models.Commands;

namespace NeuralJourney.Logic.Commands.Interfaces
{
    public interface ICommandStrategy
    {
        Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken = default);
    }
}