using NeuralJourney.Library.Models.Commands;

namespace NeuralJourney.Logic.Commands.Interfaces
{
    public interface ICommandDispatcher
    {
        public void DispatchCommand(CommandContext context);
    }
}