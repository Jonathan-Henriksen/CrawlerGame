using CrawlerGame.Logic.Commands.Interfaces;

namespace CrawlerGame.Logic.Commands.PlayerCommands
{
    internal class UnknownCommand : ICommand
    {
        void ICommand.Execute()
        {
            Console.WriteLine("Unknown command");
        }
    }
}