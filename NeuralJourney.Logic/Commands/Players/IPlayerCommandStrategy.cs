using NeuralJourney.Library.Models.World;

namespace NeuralJourney.Logic.Commands.Players
{
    public interface IPlayerCommandStrategy
    {
        Task ExecuteAsync(string playerInput, Player player);
    }
}