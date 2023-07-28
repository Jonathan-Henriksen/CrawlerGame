using CrawlerGame.Library.Models;
using CrawlerGame.Library.Models.Player;

namespace CrawlerGame.Client
{
    public class Program
    {
        private static GameInstance? Game;
        public static void Main()
        {
            Console.Write("Please enter you name -> ");
            var name = Console.ReadLine();

            Game = new GameInstance(new Character(name));

            Game.Write($"Hello {Game.Character.Name}! How are you today?");
            var answer = Game.GetPlayerInput();

            // Get response from ChatGPT based on the users answer.
            Game.Write($"ChatGPT response based on answer: {answer}");
            Game.Write("Lets get started!");

            while (Game.IsRunning())
            {
                var input = Game.GetPlayerInput();

                // Map the input to a command using ChatGPT

                if (input == "exit")
                {
                    Game.Stop();
                    continue;
                }

                Game.Write($"Sorry, \"{input}\" have not been implemented yet");
            }
        }
    }
}