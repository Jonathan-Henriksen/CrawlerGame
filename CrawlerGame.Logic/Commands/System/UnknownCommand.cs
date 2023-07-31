using CrawlerGame.Logic.Commands.Interfaces;

namespace CrawlerGame.Logic.Commands.System
{
    internal class UnknownCommand : ICommand
    {
        public bool Execute()
        {
            Console.WriteLine("Unknown command");
            return true;
        }
    }
}