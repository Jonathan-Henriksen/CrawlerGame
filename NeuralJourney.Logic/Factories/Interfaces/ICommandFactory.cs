using NeuralJourney.Library.Models.OpenAI;
using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Commands.Base;
using NeuralJourney.Logic.Engines.Interfaces;

namespace NeuralJourney.Logic.Factories.Interfaces
{
    public interface ICommandFactory
    {
        internal Command GetPlayerCommand(Player player, CommandInfo commandInfo);

        internal Command GetAdminCommand(IGameEngine game, string adminInput, params string[] parameters);
    }
}