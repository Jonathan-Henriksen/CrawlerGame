using System.Net.Sockets;

namespace NeuralJourney.Logic.Handlers.Interfaces
{
    public interface IPlayerHandler
    {
        void HandlePlayer(TcpClient playerClient, CancellationToken cancellation = default);
    }
}