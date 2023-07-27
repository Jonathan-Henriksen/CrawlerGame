namespace CrawlerGame.Library.Models
{
    internal class Room
    {
        internal Room(int xCoordinate, int yCoordinate)
        {
            Coordinates = new Coordinates()
            {
                X = xCoordinate,
                Y = yCoordinate
            };

            IsDiscovered = false;
            LightIsOn = true;
        }

        internal Door? DoorEast { get; set; }

        internal Door? DoorNorth { get; set; }

        internal Door? DoorSouth { get; set; }

        internal Door? DoorWest { get; set; }

        internal Coordinates Coordinates { get; set; }

        internal bool IsDiscovered { get; set; }

        internal bool LightIsOn { get; set; }
    }
}