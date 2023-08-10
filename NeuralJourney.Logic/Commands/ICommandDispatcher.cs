using NeuralJourney.Library.Models.Commands;

namespace NeuralJourney.Logic.Commands
{
    public interface ICommandDispatcher
    {
        public void DispatchCommand(CommandContext context);
    }
}