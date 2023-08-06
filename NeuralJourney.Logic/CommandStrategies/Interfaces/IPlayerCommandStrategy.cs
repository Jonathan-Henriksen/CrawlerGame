using NeuralJourney.Library.Models.World;

namespace NeuralJourney.Logic.CommandStrategies.Interfaces
{
    public interface IPlayerCommandStrategy
    {
        Task ExecuteAsync(string playerInput, Player player);
    }
}