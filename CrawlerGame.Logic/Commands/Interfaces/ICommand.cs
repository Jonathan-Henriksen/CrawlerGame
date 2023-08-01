namespace CrawlerGame.Logic.Commands.Interfaces
{
    public interface ICommand
    {
        internal Task<bool> ExecuteAsync();
    }
}