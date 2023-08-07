using NeuralJourney.Library.Models.World;

namespace NeuralJourney.Logic.Handlers
{
    public interface IInputHandler
    {
        public event Action<string>? OnAdminInputReceived;

        public event Action<string, Player>? OnPlayerInputReceived;

        internal Task HandleAdminInputAsync();

        internal Task HandlePlayerInputAsync(Player player);
    }
}