using CrawlerGame.Library.Models.ChatGPT;

namespace CrawlerGame.Logic.Services.Interfaces
{
    public interface IOpenAIService
    {
        public Task<CommandInfo> GetCommandFromPlayerInput(string userinput, IEnumerable<string> commands);
    }
}