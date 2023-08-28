using System.Net.Sockets;

namespace NeuralJourney.Core.Interfaces.Handlers
{
    public interface IConnectionHandler : IDisposable
    {
        public event Action<TcpClient> OnConnected;

        public Task HandleConnectionsAsync(CancellationToken cancellationToken = default);
    }
}