using System.Net.Sockets;
using System.Text;

namespace NeuralJourney.Library.Extensions
{
    public static class StreamExtensions
    {
        private static Dictionary<Stream, SemaphoreSlim> _streamSemaphores = new Dictionary<Stream, SemaphoreSlim>();

        public static async Task SendMessageAsync(this NetworkStream? stream, string message)
        {
            if (stream is null)
                return;

            var semaphore = GetSemaphore(stream);
            try
            {
                await semaphore.WaitAsync();

                var data = Encoding.UTF8.GetBytes(message);
                await stream.WriteAsync(data);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public static async Task<string?> ReadMessageAsync(this NetworkStream stream)
        {
            var buffer = new byte[4096];

            var bytesRead = await stream.ReadAsync(buffer);

            if (bytesRead == 1 && buffer[0] == 0)
                return default;

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

        private static SemaphoreSlim GetSemaphore(Stream stream)
        {
            lock (_streamSemaphores)
            {
                if (!_streamSemaphores.TryGetValue(stream, out var semaphore))
                {
                    semaphore = new SemaphoreSlim(1);
                    _streamSemaphores[stream] = semaphore;
                }
                return semaphore;
            }
        }
    }
}