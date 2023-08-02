using NeuralJourney.Library.Extensions;
using System.Net;
using System.Net.Sockets;
using Timer = System.Timers.Timer;

namespace NeuralJourney.Library.Models.World
{
    public class Player
    {
        private readonly string _ip;
        private readonly NetworkStream _stream;
        private readonly Timer _heartbeatTimer;

        public Player(TcpClient client)
        {
            _stream = client.GetStream();
            _ip = ((IPEndPoint?) client.Client.RemoteEndPoint)?.Address.ToString() ?? string.Empty;

            _heartbeatTimer = new Timer(5000);
            _heartbeatTimer.Elapsed += (sender, args) => HeartbeatTimer_Elapsed();
            _heartbeatTimer.Start();

            IsConnected = true;
            Name = $"Player({_ip})";
            Health = 100;
            Thirst = 100;
        }

        public bool IsConnected { get; private set; }

        public string Name { get; set; }

        public Coordinates? Location { get; set; }

        public int Health { get; set; }

        public int Hunger { get; set; }

        public int Thirst { get; set; }

        public NetworkStream GetStream()
        {
            return _stream;
        }

        private void HeartbeatTimer_Elapsed()
        {
            IsConnected = _stream.IsConnected();
        }
    }
}