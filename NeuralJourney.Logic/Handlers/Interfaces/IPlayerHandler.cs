using System.Net.Sockets;

namespace NeuralJourney.Logic.Handlers.Interfaces
{
    public interface IPlayerHandler
    {
        void AddPlayer(TcpClient playerClient);
    }
}