using CrawlerGame.Library.Enums;

namespace CrawlerGame.Library.Models
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

        internal Coordinates Location { get; set; }

        public Directions Direction { get; set; }
    }
}