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

            var messageData = GetMessageData(message, stream);

            var messageBytes = _encoding.GetBytes(message);
            var lengthBytes = BitConverter.GetBytes(messageBytes.Length);

            var semaphore = GetSemaphore(client);
            try
            {
                await semaphore.WaitAsync(cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                await stream.WriteAsync(lengthBytes, cancellationToken);
                await stream.WriteAsync(messageBytes, cancellationToken);

                _logger.Debug("Sent message {@MessageData}", messageData);
            }
            catch (OperationCanceledException ex)
            {
                _logger.Debug(ex, "Cancelled sending message {@MessageData}", messageData);
                throw;
            }
            catch (IOException ex)
            {
                _logger.Debug(ex, "Trouble writing to the stream {@MessageData}", messageData);
                throw; // Add retry logic
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to send message {@MessageData}", messageData);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<string> ReadMessageAsync(TcpClient client, CancellationToken cancellationToken = default)
        {
            var stream = client.GetStream();

            try
            {
                var lengthBytes = new byte[4];
                await stream.ReadAsync(lengthBytes, cancellationToken);

                var messageLength = BitConverter.ToInt32(lengthBytes, 0);
                var messageBytes = new byte[messageLength];
                await stream.ReadAsync(messageBytes.AsMemory(0, messageLength), cancellationToken);

                var message = _encoding.GetString(messageBytes);

                var messageData = GetMessageData(message, stream);
                _logger.Debug("Read message {@MessageData}", messageData);

                return message;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (IOException)
            {
                throw; // Add retry logic
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to read message from stream {Address}", GetStreamIp(stream));
                return string.Empty;
            }
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

        private static object GetMessageData(string? message, NetworkStream stream)
        {
            return new { Message = message, Address = GetStreamIp(stream) };
        }

        private static string GetStreamIp(NetworkStream stream)
        {

            return ((IPEndPoint?) stream.Socket.RemoteEndPoint)?.Address?.ToString()?.Replace("::ffff:", "") ?? "N/A";
        }
    }
}