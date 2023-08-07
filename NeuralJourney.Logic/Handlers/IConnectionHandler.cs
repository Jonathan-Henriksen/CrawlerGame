using NeuralJourney.Library.Models.World;

namespace NeuralJourney.Logic.Handlers
{
    public interface IConnectionHandler
    {
        internal event Action<Player> OnPlayerConnected;

        internal Task HandleConnectionsAsync();

        internal void Stop();
    }
}