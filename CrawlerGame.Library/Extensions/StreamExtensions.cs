using System.Net.Sockets;
using System.Text;

namespace CrawlerGame.Library.Extensions
{
    public static class StreamExtensions
    {
        public static async Task SendMessageAsync(this NetworkStream? stream, string message)
        {
            if (stream is null)
                return;

            var data = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(data);
        }

        public static async Task<string> ReadMessageAsync(this NetworkStream stream)
        {
            var buffer = new byte[4096];

            var bytesRead = await stream.ReadAsync(buffer);
            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }

        public static bool IsConnected(this NetworkStream? stream)
        {
            if (stream is null)
                return false;

            var result = false;

            var streamReader = new StreamReader(stream);
            var timeout = streamReader.BaseStream.ReadTimeout;

            try
            {
                streamReader.BaseStream.ReadTimeout = 250;
                streamReader.BaseStream.WriteByte(0);
                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                streamReader.BaseStream.ReadTimeout = timeout;
            }

            return result;
        }
    }
}