using CrawlerGame.Library.Models.Player;
using CrawlerGame.Library.Models.World;
using Newtonsoft.Json;

namespace CrawlerGame.Library.Models
{
    public class GameInstance
    {
        public GameInstance(string characterName, string gamemasterName)
        {
            GameMasterName = gamemasterName;

            Rooms = BuildRooms();

            Character = new Character(characterName, Rooms[0, 0]);

            Start();
        }

        private string GameMasterName { get; set; }

        private bool GameIsRunning { get; set; }

        private Room[,] Rooms { get; set; }

        internal Character Character { get; set; }

        public bool IsRunning()
        {
            return GameIsRunning;
        }

        private void Start()
        {
            GameIsRunning = true;
        }

        public void Stop()
        {
            GameIsRunning = false;
        }

        public void Say(string text)
        {
            Console.WriteLine($"\n{GameMasterName} -> {text}");
        }

        public string GetPlayerInput()
        {
            Console.Write($"\n{Character.Name} -> ");
            return Console.ReadLine() ?? string.Empty;
        }

        public string GetPlayerName()
        {
            return Character.Name;
        }

        private static Room[,] BuildRooms()
        {
            var roomsJson = File.ReadAllText("rooms.json") ?? string.Empty;
            var rooms = JsonConvert.DeserializeObject<Room[,]>(roomsJson) ?? new Room[1, 1];

            int width = rooms.GetLength(0);
            int height = rooms.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var currentRoom = rooms[x, y];

                    if (currentRoom == null)
                        continue;

                    if (x > 0)
                    {
                        currentRoom.DoorWest = new Door(rooms[x - 1, y], Enums.Directions.West);
                    }

                    if (x < width - 1)
                    {
                        currentRoom.DoorEast = new Door(rooms[x + 1, y], Enums.Directions.East);
                    }

                    if (y > 0)
                    {
                        currentRoom.DoorSouth = new Door(rooms[x, y - 1], Enums.Directions.South);
                    }

                    if (y < height - 1)
                    {
                        currentRoom.DoorNorth = new Door(rooms[x, y + 1], Enums.Directions.North);
                    }
                }
            }

            return rooms;
        }
    }
}