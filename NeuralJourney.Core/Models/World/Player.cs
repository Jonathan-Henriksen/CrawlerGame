using System.Net.Sockets;

namespace NeuralJourney.Core.Models.World
{
    public class Player
    {
        private readonly TcpClient _client;

        public readonly Guid Id;

        public Player(TcpClient client, string name, Guid id)
        {
            _client = client;

            Id = id;

            IsConnected = true;
            Name = name;
            Health = 100;
            Thirst = 100;
            Location = new Coordinates();
        }

        public bool IsConnected { get; private set; }

        public string Name { get; set; }

        public Coordinates Location { get; set; }

        public int Health { get; set; }

        public int Hunger { get; set; }

        public int Thirst { get; set; }

        public TcpClient GetClient()
        {
            return _client;
        }
    }
}