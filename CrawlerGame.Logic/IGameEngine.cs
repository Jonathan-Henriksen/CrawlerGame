using System.Net.Sockets;

namespace CrawlerGame.Logic
{
    public interface IGameEngine
    {
        public void AddPlayer(TcpClient playerClient);

        public IGameEngine Init();

        public void Start();

        public void Stop();
    }
}