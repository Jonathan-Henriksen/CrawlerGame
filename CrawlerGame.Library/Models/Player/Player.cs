using CrawlerGame.Library.Enums;
using CrawlerGame.Library.Models.World;

namespace CrawlerGame.Library.Models.Player
{
    public class Player
    {
        public Player()
        {
            Name = "Player";
            Location = new Coordinates() { X = 0, Y = 0 };
            Health = 100;
            Thirst = 100;
        }

        public string Name { get; set; }

        private Coordinates Location { get; set; }

        private int Health { get; set; }

        private int Hunger { get; set; }

        private int Thirst { get; set; }

        public bool IsHungry()
        {
            return Hunger < 50;
        }

        public bool IsThirsty()
        {
            return Thirst < 50;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    Location.Y++;
                    break;
                case Direction.South:
                    Location.Y--;
                    break;
                case Direction.East:
                    Location.X++;
                    break;
                case Direction.West:
                    Location.X--;
                    break;
                default:
                    break;
            }
        }

        public void AdjustHealth(int adjustment)
        {
            Health += adjustment;
        }
    }
}