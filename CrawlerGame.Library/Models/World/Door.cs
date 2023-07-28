using CrawlerGame.Library.Enums;

namespace CrawlerGame.Library.Models.World
{
    internal class Door
    {
        public Door(Room destination, Directions direction)
        {
            Destination = destination;
            Direction = direction;
            Locked = false;
            Closed = true;
        }

        private bool Locked { get; set; }

        private bool Closed { get; set; }

        private Directions Direction { get; set; }

        internal Room Destination { get; set; }

        private Door OppositeDoor
        {
            get
            {
                return Direction switch
                {
                    Directions.North => Destination.DoorSouth!,
                    Directions.South => Destination.DoorNorth!,
                    Directions.West => Destination.DoorEast!,
                    Directions.East => Destination.DoorWest!,
                    _ => this,
                };
            }
        }

        internal bool IsClosed()
        {
            return Closed;
        }

        internal void Open()
        {
            if (Locked || !Closed)
                return;

            Closed = false;
            OppositeDoor.Closed = false;
        }

        internal void Close()
        {
            if (Closed)
                return;

            Closed = true;
            OppositeDoor.Closed = true;
        }

        internal bool IsLocked()
        {
            return this.Locked;
        }

        internal void Lock()
        {
            if (!Closed || Locked)
                return;

            Locked = true;
            OppositeDoor.Locked = true;
        }

        internal void Unlock()
        {
            if (!Closed || !Locked)
                return;

            Locked = false;
            OppositeDoor.Locked = false;
        }
    }
}