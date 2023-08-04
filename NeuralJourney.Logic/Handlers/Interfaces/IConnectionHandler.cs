using NeuralJourney.Library.Models.World;

namespace NeuralJourney.Logic.Handlers.Interfaces
{
    public interface IConnectionHandler
    {
        internal event Action<Player> OnPlayerConnected;

        internal Task HandleAsync();

        internal void Stop();
    }
}