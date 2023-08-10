using System.Net.Sockets;

namespace NeuralJourney.Logic.Handlers.Interfaces
{
    public interface IConnectionHandler
    {
        internal event Action<TcpClient> OnConnected;

        internal Task HandleConnectionsAsync();

        internal void Stop();
    }
}