using NeuralJourney.Library.Models.Commands;
using NeuralJourney.Library.Models.World;

namespace NeuralJourney.Logic.Commands.Interfaces
{
    public interface ICommandStrategy
    {
        Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken = default);
    }
}