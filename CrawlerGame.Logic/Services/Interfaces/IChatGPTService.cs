namespace CrawlerGame.Logic.Services.Interfaces
{
    public interface IChatGPTService
    {
        public Task<string> GetCommandFromPlayerInput(string userinput);
    }
}