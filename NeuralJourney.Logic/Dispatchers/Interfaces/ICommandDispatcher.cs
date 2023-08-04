using NeuralJourney.Library.Models.CommandInfo;

namespace NeuralJourney.Logic.Dispatchers.Interfaces
{
    public interface ICommandDispatcher
    {
        internal void DispatchPlayerCommandAsync(PlayerCommandInfo commandInfo);

        internal void DispatchAdminCommandAsync(AdminCommandInfo commandInfo);
    }
}