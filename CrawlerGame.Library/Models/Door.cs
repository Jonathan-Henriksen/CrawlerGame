namespace CrawlerGame.Library.Models
{
    internal class Door
    {
        internal bool Locked { get; set; }

        internal bool Closed { get; set; }

        internal Room? Destination { get; set; }
    }
}