using CrawlerGame.Library.Models;
using CrawlerGame.Logic.Services.Interfaces;

namespace CrawlerGame.Client
{
    internal class App
    {
        private readonly IChatGPTService _chatGPTService;

        private GameInstance? Game;

        internal App(IChatGPTService chatGPTService)
        {
            _chatGPTService = chatGPTService;
        }

        internal async Task Run(string[] args)
        {
            Console.Write("Please enter you name -> ");
            var name = Console.ReadLine() ?? "Player";

            Game = new GameInstance(name, "Gamemaster");

            Game.Say($"Hello {Game.GetPlayerName()}!");
            Game.Say("Lets get started!");

            while (Game.IsRunning())
            {
                var input = Game.GetPlayerInput();

                var command = await _chatGPTService.GetCommandFromPlayerInput(input);

                Update(command);
            }
        }

        private void Update(string command)
        {
            if (command.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
            {
                Game?.Stop();
                return;
            }
        }
    }
}