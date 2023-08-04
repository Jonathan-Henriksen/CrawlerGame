using NeuralJourney.Library.Models.World;

namespace NeuralJourney.Logic.Handlers.Interfaces
{
    public interface IInputHandler
    {
        internal Task HandleAdminInputAsync();

        internal Task HandlePlayerInputAsync(Player player);
    }
}