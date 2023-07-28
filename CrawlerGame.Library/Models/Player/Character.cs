using CrawlerGame.Library.Enums;
using CrawlerGame.Library.Interfaces;
using CrawlerGame.Library.Models.World;

namespace CrawlerGame.Library.Models.Player
{
    internal class Character : IUpdateable
    {
        internal Character(string? name, Room location)
        {
            if (string.IsNullOrEmpty(name))
                name = "Unknown";

            Name = name;
            Location = location;
            Direction = Directions.North;
        }

        internal string Name { get; set; }

        internal Room Location { get; set; }

        internal Directions Direction { get; set; }

        public void Update(string command)
        {
            switch (command)
            {
                default:
                    break;
            }
        }
    }
}