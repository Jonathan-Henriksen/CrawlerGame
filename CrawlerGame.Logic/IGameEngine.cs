namespace CrawlerGame.Logic
{
    public interface IGameEngine
    {
        public IGameEngine Init();

        public void Start();

        public void Stop();

        public bool IsRunning();

        public void Update();

        public void SetPlayerName(string playerName);

        public Task ProcessPlayerInput();
    }
}