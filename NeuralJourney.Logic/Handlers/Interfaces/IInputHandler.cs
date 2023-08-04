using NeuralJourney.Library.Models.World;

namespace NeuralJourney.Logic.Handlers.Interfaces
{
    public interface IInputHandler
    {
        internal event Action<string>? OnAdminInputReceived;

        internal event Action<(Player, string)>? OnPlayerInputReceived;

        internal Task HandleAdminInputAsync();

        internal Task HandlePlayerInputAsync(Player player);
    }
}