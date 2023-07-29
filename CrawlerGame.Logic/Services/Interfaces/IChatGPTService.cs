using CrawlerGame.Library.Models.ChatGPT;

namespace CrawlerGame.Logic.Services.Interfaces
{
    public interface IChatGPTService
    {
        public Task<CommandMapperResponse?> GetCommandFromPlayerInput(string userinput, IEnumerable<string> commands);
    }
}