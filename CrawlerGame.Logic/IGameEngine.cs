namespace CrawlerGame.Logic
{
    public interface IGameEngine
    {
        public void Start();

        public void Stop();

        public bool IsRunning();

        public void Update(string userInput);

        public void SetPlayerName(string playerName);

        public string GetPlayerInput();
    }
}