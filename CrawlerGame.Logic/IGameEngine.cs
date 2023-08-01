using System.Net.Sockets;

namespace CrawlerGame.Logic
{
    public interface IGameEngine
    {
        

        public IGameEngine Init();

        public void Start();

        public void Stop();

        public void AddPlayer(TcpClient playerClient);

        public Task ExecuteAdminCommandAsync(string adminInput);
    }
}