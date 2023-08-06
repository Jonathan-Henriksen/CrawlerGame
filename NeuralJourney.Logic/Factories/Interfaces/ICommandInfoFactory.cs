using NeuralJourney.Library.Models.CommandInfo;
using NeuralJourney.Library.Models.World;

namespace NeuralJourney.Logic.Factories.Interfaces
{
    public interface ICommandInfoFactory
    {
        internal Task<AdminCommandInfo> CreateAdminCommandInfoFromInputAsync(string input);

        internal Task<PlayerCommandInfo> CreatePlayerCommandInfoFromInputAsync(string input, Player player);
    }
}