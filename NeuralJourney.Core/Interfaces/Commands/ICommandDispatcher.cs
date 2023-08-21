using NeuralJourney.Core.Models.LogProperties;

namespace NeuralJourney.Core.Interfaces.Commands
{
    public interface ICommandDispatcher
    {
        Task DispatchCommandAsync(CommandContext context);
    }
}