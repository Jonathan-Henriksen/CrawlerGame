using NeuralJourney.Library.Models.World;

namespace NeuralJourney.Logic.Dispatchers.Interfaces
{
    public interface ICommandDispatcher
    {
        internal void DispatchPlayerCommandAsync((Player Player, string Text) playerinput);

        internal void DispatchAdminCommandAsync(string input);
    }
}