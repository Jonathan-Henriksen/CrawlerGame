using NeuralJourney.Core.Interfaces.Services;
using Serilog;
using System.Text;

namespace NeuralJourney.Infrastructure.Services
{
    public class MessageService : IMessageService
    {
        private readonly Dictionary<Stream, SemaphoreSlim> _streamSemaphores = new();
        private readonly Encoding _encoding = Encoding.UTF8;
        private readonly ILogger _logger;

        public const string CloseConnectionMessage = "__CLOSE_CONNECTION__";

        public MessageService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task SendMessageAsync(Stream stream, string message, CancellationToken cancellationToken = default)
        {
            var messageBytes = _encoding.GetBytes(message);
            var lengthBytes = BitConverter.GetBytes(messageBytes.Length);

            var semaphore = GetSemaphore(stream);
            try
            {
                await semaphore.WaitAsync(cancellationToken);

                await stream.WriteAsync(lengthBytes, cancellationToken);
                await stream.WriteAsync(messageBytes, cancellationToken);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<string> ReadMessageAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            var lengthBytes = new byte[4];
            await stream.ReadAsync(lengthBytes, cancellationToken);

            var messageLength = BitConverter.ToInt32(lengthBytes, 0);
            var messageBytes = new byte[messageLength];
            await stream.ReadAsync(messageBytes.AsMemory(0, messageLength), cancellationToken);

            return _encoding.GetString(messageBytes);
        }

        public async Task SendCloseConnectionAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            _logger.Debug("Sending 'Close Connection' message");

            await SendMessageAsync(stream, CloseConnectionMessage, cancellationToken);
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