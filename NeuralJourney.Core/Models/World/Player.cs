using System.Net;
using System.Net.Sockets;

namespace NeuralJourney.Core.Models.World
{
    public class Player
    {
        private readonly string _ip;
        private readonly NetworkStream _stream;

        public readonly Guid ID;

        public Player(TcpClient client)
        {
            _stream = client.GetStream();
            _ip = ((IPEndPoint?) client.Client.RemoteEndPoint)?.Address.ToString() ?? string.Empty;

            ID = Guid.NewGuid();

            IsConnected = true;
            Name = $"Player({_ip})";
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

        public NetworkStream GetStream()
        {
            return _stream;
        }
    }
}