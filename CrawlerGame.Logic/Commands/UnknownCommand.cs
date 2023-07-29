using CrawlerGame.Logic.Commands.Interfaces;

namespace CrawlerGame.Logic.Commands
{
    internal class UnknownCommand : ICommand
    {
        void ICommand.Execute()
        {
            throw new NotImplementedException();
        }
    }
}