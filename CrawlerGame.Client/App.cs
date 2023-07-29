using CrawlerGame.Logic;

namespace CrawlerGame.Client
{
    internal class App
    {
        private readonly IGameEngine _gameEngine;

        internal App(IGameEngine gameEngine)
        {
            _gameEngine = gameEngine;
        }

        internal void Run(string[] args)
        {
            Console.Write("Please enter you name -> ");
            var name = Console.ReadLine() ?? "Player";

            _gameEngine.SetPlayerName(name);

            _gameEngine.Start();

            while (_gameEngine.IsRunning())
            {
                var input = _gameEngine.GetPlayerInput();

                _gameEngine.Update(input);
            }
        }
    }
}