using CrawlerGame.Library.Enums;
using CrawlerGame.Library.Models.World;

namespace CrawlerGame.Library.Models.Player
{
    public class Character
    {
        public Character(string? name)
        {
            if (string.IsNullOrEmpty(name))
                name = "Unknown";

            Name = name;
            Location = new Coordinates() { X = 0, Y = 0 };
            Direction = Directions.North;
        }

        public string Name { get; set; }

        private Coordinates Location { get; set; }

        private Directions Direction { get; set; }
    }
}