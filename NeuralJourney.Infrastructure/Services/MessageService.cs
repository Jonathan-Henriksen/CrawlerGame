using NeuralJourney.Core.Interfaces.Services;
using Serilog;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NeuralJourney.Infrastructure.Services
{
    public class MessageService : IMessageService
    {
        private readonly Dictionary<TcpClient, SemaphoreSlim> _streamSemaphores = new();
        private readonly Encoding _encoding = Encoding.UTF8;
        private readonly ILogger _logger;

        public const string CloseConnectionMessage = "__CLOSE_CONNECTION__";

        public MessageService(ILogger logger)
        {
            _logger = logger.ForContext<MessageService>();
        }

        public async Task SendMessageAsync(TcpClient client, string message, CancellationToken cancellationToken = default)
        {
            var stream = client.GetStream();

            var messageData = GetMessageData(message, client);

            if (!stream.CanWrite)
            {
                _logger.Error("Failed to send message {@Reason} {@MessageData}", messageData, "Could not write to stream");
            }

            _logger.Debug("Sending message {@MessageData}", messageData);

            var messageBytes = _encoding.GetBytes(message);
            var lengthBytes = BitConverter.GetBytes(messageBytes.Length);

            cancellationToken.ThrowIfCancellationRequested();

            var semaphore = GetSemaphore(client);
            try
            {
                await semaphore.WaitAsync(cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                await stream.WriteAsync(lengthBytes, cancellationToken);
                await stream.WriteAsync(messageBytes, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to send message {@MessageData}", messageData);
            }
            finally
            {
                _logger.Debug("Sent message {@MessageData}", messageData);
                semaphore.Release();
            }
        }

        public async Task<string> ReadMessageAsync(TcpClient client, CancellationToken cancellationToken = default)
        {
            var stream = client.GetStream();

            var lengthBytes = new byte[4];
            await stream.ReadAsync(lengthBytes, cancellationToken);

            var messageLength = BitConverter.ToInt32(lengthBytes, 0);
            var messageBytes = new byte[messageLength];
            await stream.ReadAsync(messageBytes.AsMemory(0, messageLength), cancellationToken);

            var message = _encoding.GetString(messageBytes);

            var messageData = GetMessageData(message, client);
            _logger.Debug("Read message {@MessageData}", messageData);

            return message;
        }

        public async Task SendCloseConnectionAsync(TcpClient client, CancellationToken cancellationToken = default)
        {
            await SendMessageAsync(client, CloseConnectionMessage, cancellationToken);
        }

        public bool IsCloseConnectionMessage(string message)
        {
            return message == CloseConnectionMessage;
        }

        private SemaphoreSlim GetSemaphore(TcpClient client)
        {
            lock (_streamSemaphores)
            {
                if (!_streamSemaphores.TryGetValue(client, out var semaphore))
                {
                    semaphore = new SemaphoreSlim(1);
                    _streamSemaphores[client] = semaphore;
                }
                return semaphore;
            }
        }

        private static object GetMessageData(string? message, TcpClient client)
        {
            return new { Message = message, Address = ((IPEndPoint?) client.Client.RemoteEndPoint)?.Address.ToString() ?? string.Empty };
        }
    }
}