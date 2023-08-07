using NeuralJourney.Library.Models.World;

namespace NeuralJourney.Logic.Commands
{
    public interface ICommandDispatcher
    {
        internal void DispatchAdminCommand(string adminInput);

        internal void DispatchPlayerCommand(string playerInput, Player player);
    }
}