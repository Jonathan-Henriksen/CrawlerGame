namespace CrawlerGame.Logic.Services.Interfaces
{
    public interface IChatGPTService
    {
        public Task<string> GetCommandFromUserinput(string userinput);
    }
}