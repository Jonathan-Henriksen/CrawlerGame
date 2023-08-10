using System.Net.Sockets;

namespace NeuralJourney.Logic.Handlers
{
    public interface IPlayerHandler
    {
        void AddPlayer(TcpClient playerClient);
    }
}