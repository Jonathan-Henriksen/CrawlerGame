using NeuralJourney.Core.Models.Commands;

namespace NeuralJourney.Core.Interfaces.Commands
{
    public interface ICommandDispatcher
    {
        Task DispatchCommandAsync(CommandContext context);
    }
}