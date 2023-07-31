using CrawlerGame.Library.Models.World;

namespace CrawlerGame.Library.Models.Player
{
    public class Player
    {
        private readonly string _ip;

        public Player(string ip)
        {
            _ip = ip;

            Name = $"Player({_ip})";
            Health = 100;
            Thirst = 100;
        }

        public string Name { get; set; }

        public Coordinates? Location { get; set; }

        public int Health { get; set; }

        public int Hunger { get; set; }

        public int Thirst { get; set; }
    }
}