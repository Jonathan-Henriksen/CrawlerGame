using System.Net.Sockets;

namespace NeuralJourney.Core.Interfaces.Handlers
{
    public interface IPlayerHandler : IDisposable
    {
        void AddPlayer(TcpClient playerClient, CancellationToken cancellation = default);

        Task RemoveAllPlayers();
    }
}