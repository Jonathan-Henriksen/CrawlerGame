using System.Net.Sockets;

namespace NeuralJourney.Core.Interfaces.Handlers
{
    public interface IConnectionHandler
    {
        public event Action<TcpClient> OnConnected;

        public Task HandleConnectionsAsync(CancellationToken cancellationToken = default);

        public void Stop();
    }
}