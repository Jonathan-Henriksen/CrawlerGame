using NeuralJourney.Library.Models.ChatGPT;
using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Commands.Base;

namespace NeuralJourney.Logic.Factories.Interfaces
{
    public interface ICommandFactory
    {
        internal Command GetPlayerCommand(Player player, CommandInfo commandInfo);

        internal Command GetAdminCommand(IGameEngine game, string adminInput, params string[] parameters);
    }
}