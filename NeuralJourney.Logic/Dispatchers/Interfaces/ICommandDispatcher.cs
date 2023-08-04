using NeuralJourney.Library.Models.CommandInfo;

namespace NeuralJourney.Logic.Dispatchers.Interfaces
{
    public interface ICommandDispatcher
    {
        internal Task DispatchPlayerCommandAsync(PlayerCommandInfo commandInfo);

        internal Task DispatchAdminCommandAsync(AdminCommandInfo commandInfo);
    }
}