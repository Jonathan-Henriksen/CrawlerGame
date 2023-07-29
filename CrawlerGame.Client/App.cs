using CrawlerGame.Logic;

namespace CrawlerGame.Client
{
    public class App
    {
        private readonly IGameEngine _gameEngine;

        public App(IGameEngine gameEngine)
        {
            _gameEngine = gameEngine;
        }

        public void Run(string[] args)
        {
            _gameEngine.Init().Start();

            while (_gameEngine.IsRunning())
            {
                _gameEngine.ProcessPlayerInput();

                _gameEngine.Update();
            }
        }
    }
}