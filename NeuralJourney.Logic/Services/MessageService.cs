using System.Text;

namespace NeuralJourney.Logic.Services
{
    public class MessageService : IMessageService
    {
        private readonly Dictionary<Stream, SemaphoreSlim> _streamSemaphores = new();
        private readonly Encoding _encoding = Encoding.UTF8;

        public const string CloseConnectionMessage = "__CLOSE_CONNECTION__";

        public async Task SendMessageAsync(Stream stream, string message)
        {
            byte[] messageBytes = _encoding.GetBytes(message);
            byte[] lengthBytes = BitConverter.GetBytes(messageBytes.Length);

            var semaphore = GetSemaphore(stream);
            try
            {
                await semaphore.WaitAsync();

                await stream.WriteAsync(lengthBytes);
                await stream.WriteAsync(messageBytes);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<string> ReadMessageAsync(Stream stream)
        {
            byte[] lengthBytes = new byte[4];
            await stream.ReadAsync(lengthBytes);

            int messageLength = BitConverter.ToInt32(lengthBytes, 0);
            byte[] messageBytes = new byte[messageLength];
            await stream.ReadAsync(messageBytes.AsMemory(0, messageLength));

            return _encoding.GetString(messageBytes);
        }

        public async Task SendCloseConnectionAsync(Stream stream)
        {
            await SendMessageAsync(stream, CloseConnectionMessage);
        }

        public bool IsCloseConnectionMessage(string message)
        {
            return message == CloseConnectionMessage;
        }

        private SemaphoreSlim GetSemaphore(Stream stream)
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