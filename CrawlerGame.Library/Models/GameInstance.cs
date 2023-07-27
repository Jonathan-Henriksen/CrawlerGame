using Newtonsoft.Json;

namespace CrawlerGame.Library.Models
{
    public class GameInstance
    {
        public GameInstance(Character character)
        {
            var roomsJson = File.ReadAllText("rooms.json") ?? string.Empty;

            Rooms = JsonConvert.DeserializeObject<Room[,]>(roomsJson) ?? new Room[1, 1];
            GameMasterName = "Gamemaster";
            Character = character;

            LinkRooms();
            Start();
        }

        private string GameMasterName { get; set; }

        private bool GameIsRunning { get; set; }

        private Room[,] Rooms { get; set; }

        public Character Character { get; set; }

        public bool IsRunning()
        {
            return GameIsRunning;
        }

        public void Start()
        {
            GameIsRunning = true;
        }

        public void Stop()
        {
            GameIsRunning = false;
        }

        public void Write(string text)
        {
            Console.WriteLine($"\n{GameMasterName} -> {text}");
        }

        public string GetUserInput()
        {
            Console.Write($"\n{Character.Name} -> ");
            return Console.ReadLine() ?? string.Empty;
        }

        private void LinkRooms()
        {
            int width = Rooms.GetLength(0);
            int height = Rooms.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var currentRoom = Rooms[x, y];

                    if (currentRoom == null)
                        continue;

                    if (currentRoom.DoorNorth != null && currentRoom.Coordinates.Y > 0)
                    {
                        currentRoom.DoorNorth.Destination = Rooms[x, y - 1];
                    }

                    if (currentRoom.DoorSouth != null && currentRoom.Coordinates.Y < height - 1)
                    {
                        currentRoom.DoorSouth.Destination = Rooms[x + 1, y];
                    }

                    if (currentRoom.DoorWest != null && currentRoom.Coordinates.X > 0)
                    {
                        currentRoom.DoorWest.Destination = Rooms[x - 1, y];
                    }

                    if (currentRoom.DoorEast != null && currentRoom.Coordinates.X < width - 1)
                    {
                        currentRoom.DoorEast.Destination = Rooms[x + 1, y];
                    }
                }
            }
        }
    }
}