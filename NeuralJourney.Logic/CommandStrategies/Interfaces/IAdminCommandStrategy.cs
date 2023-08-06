using NeuralJourney.Library.Models.CommandInfo;

namespace NeuralJourney.Logic.CommandStrategies.Interfaces
{
    public interface IAdminCommandStrategy
    {
        Task ExecuteAsync(string adminInput);
    }
}