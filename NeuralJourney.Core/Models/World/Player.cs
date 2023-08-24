using NeuralJourney.Core.Models.LogProperties;
using System.Net.Sockets;

namespace NeuralJourney.Core.Models.World
{
    public class Player
    {
        public TcpClient Client { get; }
        public Guid Id { get; }
        public string Name { get; }

        public bool HasIncompleteCommand { get; set; }

        public Stack<CommandContext> PreviousCommands { get; set; }

        public int Health { get; set; } = 100;
        public int Hunger { get; set; } = 100;
        public int Thirst { get; set; } = 100;
        public Coordinates Location { get; set; } = new Coordinates();

        public Player(TcpClient client, string name, Guid id)
        {
            Client = client;
            Id = id;
            Name = name;

            PreviousCommands = new Stack<CommandContext>();
        }
    }
}