using CrawlerGame.Library.Models;

namespace CrawlerGame.Client
{
    public class Program
    {
        private static GameInstance? Game;
        public static void Main()
        {
            Console.Write($"Enter you name -> ");
            var name = Console.ReadLine();

            Game = new GameInstance(new Character(name));

            Game.Write($"Hello {Game.Character.Name}! How are you today?");
            var answer = Game.GetUserInput();

            // Get response from ChatGPT based on the users answer.
            Game.Write($"ChatGPT response based on answer: {answer}");
            Game.Write("Lets get started!");

            while (Game.IsRunning())
            {
                var input = Game.GetUserInput();

                // Map the input to a command using ChatGPT

                if (input == "exit")
                {
                    Game.Stop();
                    continue;
                }

                Game.Write($"Sorry I don't understand what \"{input}\" means");
            }
        }
    }
}