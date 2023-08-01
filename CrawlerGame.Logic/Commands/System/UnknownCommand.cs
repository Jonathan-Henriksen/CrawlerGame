using CrawlerGame.Logic.Commands.Interfaces;

namespace CrawlerGame.Logic.Commands.System
{
    internal class UnknownCommand : ICommand
    {
        public Task<bool> ExecuteAsync()
        {
            Console.WriteLine("Unknown command");
            return Task.FromResult(true);
        }
    }
}