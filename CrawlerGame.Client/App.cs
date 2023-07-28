using CrawlerGame.Library.Models.Player;
using CrawlerGame.Library.Models;

namespace CrawlerGame.Client
{
    internal class App
    {
        private static GameInstance? Game;

        internal static void Run(string[] args)
        {
            Console.Write("Please enter you name -> ");
            var name = Console.ReadLine();

            Game = new GameInstance(new Character(name), "Gamemaster");

            Game.Write($"Hello {Game.Character.Name}! How are you today?");
            var answer = Game.GetPlayerInput();

            // Get response from ChatGPT based on the users answer.
            Game.Write($"ChatGPT response based on answer: {answer}");
            Game.Write("Lets get started!");

            while (Game.IsRunning())
            {
                var input = Game.GetPlayerInput();

                Update(input);

                Game.Write($"Sorry, \"{input}\" have not been implemented yet");
            }
        }

        private static void Update(string userInput)
        {
            if (userInput == "exit")
            {
                Game?.Stop();
                return;
            }

            // Do stuff
        }
    }
}