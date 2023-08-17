using System.Net;
using System.Net.Sockets;

namespace NeuralJourney.Core.Extensions
{
    public static class TcpClientExtensions
    {
        public static string GetLocalIp(this TcpClient client)
        {
            return client.Client.LocalEndPoint is IPEndPoint endPoint ? endPoint.Address.ToString().Replace("::ffff:", "") : "N/A";
        }

        public static string GetRemoteIp(this TcpClient client)
        {
            return client.Client.RemoteEndPoint is IPEndPoint endPoint ? endPoint.Address.ToString().Replace("::ffff:", "") : "N/A";
        }
    }
}