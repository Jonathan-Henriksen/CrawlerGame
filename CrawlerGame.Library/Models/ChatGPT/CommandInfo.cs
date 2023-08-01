using CrawlerGame.Library.Enums;
using CrawlerGame.Library.Models.World;

namespace CrawlerGame.Library.Models.ChatGPT
{
    public class CommandInfo
    {
        public CommandEnum Command { get; set; }

        public string Param { get; set; } = string.Empty;

        public string SuccessMessage { get; set; } = string.Empty;

        public string FailureMessage { get; set; } = string.Empty;

        public Player? Player { get; set; }
    }
}