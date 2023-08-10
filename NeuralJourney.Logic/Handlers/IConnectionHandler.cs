using System.Net.Sockets;

namespace NeuralJourney.Logic.Handlers
{
    public interface IConnectionHandler
    {
        internal event Action<TcpClient> OnConnected;

        internal Task HandleConnectionsAsync();

        internal void Stop();
    }
}