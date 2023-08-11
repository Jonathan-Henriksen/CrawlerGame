using NeuralJourney.Core.Commands;

namespace NeuralJourney.Core.Interfaces.Commands
{
    public interface ICommandDispatcher
    {
        public void DispatchCommand(CommandContext context);
    }
}