using CrawlerGame.Library.Enums;

namespace CrawlerGame.Library.Models.ChatGPT
{
    public class CommandMapperResponse
    {
        public CommandEnum Command { get; set; }

        public string Param { get; set; } = string.Empty;

        public string CommandText { get; set; } = string.Empty;
    }
}