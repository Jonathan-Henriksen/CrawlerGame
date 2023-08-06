using NeuralJourney.Library.Models.World;

namespace NeuralJourney.Logic.Handlers.Interfaces
{
    public interface IConnectionHandler
    {
        internal event Action<Player> OnPlayerConnected;

        internal Task HandleConnectionsAsync();

        internal void Stop();
    }
}