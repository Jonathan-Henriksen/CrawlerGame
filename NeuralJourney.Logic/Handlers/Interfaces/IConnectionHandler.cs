using NeuralJourney.Library.Models.World;

namespace NeuralJourney.Logic.Handlers.Interfaces
{
    public interface IConnectionHandler
    {
        public event Action<Player> Connected;

        public Task HandleAsync();

        public void Stop();
    }
}