using System.Net.Sockets;

namespace NeuralJourney.Library.Extensions
{
    public static class StreamExtensions
    {
        private static readonly Dictionary<Stream, SemaphoreSlim> _streamSemaphores = new();

        public static async Task SendMessageAsync(this NetworkStream? stream, string message)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            var semaphore = GetSemaphore(stream);
            try
            {
                await semaphore.WaitAsync();

                using var writer = new StreamWriter(stream, leaveOpen: true);

                await writer.WriteAsync(message);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public static async Task<string> ReadMessageAsync(this NetworkStream stream)
        {
            using var reader = new StreamReader(stream, leaveOpen: true);

            return await reader.ReadLineAsync() ?? string.Empty;
        }

        public static bool IsConnected(this NetworkStream? stream)
        {
            if (stream is null)
                return false;

            using var streamReader = new StreamReader(stream, leaveOpen: true);
            var timeout = streamReader.BaseStream.ReadTimeout;

            try
            {
                streamReader.BaseStream.ReadTimeout = 250;
                streamReader.BaseStream.WriteByte(0);
                return true;
            }
            catch
            {
                return false;
            }
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