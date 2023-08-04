using NeuralJourney.Library.Models.CommandInfo;
using NeuralJourney.Library.Models.World;

namespace NeuralJourney.Logic.Handlers.Interfaces
{
    public interface IInputHandler
    {
        public event Action<AdminCommandInfo>? OnAdminInputReceived;

        public event Action<PlayerCommandInfo>? OnPlayerInputReceived;

        internal Task HandleAdminInputAsync();

        internal Task HandlePlayerInputAsync(Player player);
    }
}