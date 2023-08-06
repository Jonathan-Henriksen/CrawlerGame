using NeuralJourney.Library.Models.CommandInfo;
using NeuralJourney.Library.Models.World;

namespace NeuralJourney.Logic.Handlers.Interfaces
{
    public interface IInputHandler
    {
        public event Action<string>? OnAdminInputReceived;

        public event Action<(Player Player, string PlayerInput)>? OnPlayerInputReceived;

        internal Task HandleAdminInputAsync();

        internal Task HandlePlayerInputAsync(Player player);
    }
}