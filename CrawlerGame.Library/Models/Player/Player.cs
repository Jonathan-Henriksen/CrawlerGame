using CrawlerGame.Library.Extensions;
using CrawlerGame.Library.Models.World;
using System.Net;
using System.Net.Sockets;
using Timer = System.Timers.Timer;

namespace CrawlerGame.Library.Models.Player
{
    public class Player
    {
        private readonly string _ip;
        private readonly TcpClient _tcpClient;
        private readonly Timer _heartbeatTimer;

        public Player(TcpClient playerClient)
        {
            _tcpClient = playerClient;
            _ip = ((IPEndPoint?) _tcpClient.Client.RemoteEndPoint)?.Address.ToString() ?? string.Empty;

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

        public NetworkStream? GetStream()
        {
            if (!_tcpClient.Connected)
                return default;

            return _tcpClient.GetStream();
        }

        private void HeartbeatTimer_Elapsed()
        {
            if (!_tcpClient.Connected)
            {
                IsConnected = false;
            }

            IsConnected = GetStream().IsConnected();
        }
    }
}