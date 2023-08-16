using NeuralJourney.Core.Constants.Messages;
using NeuralJourney.Core.Exceptions;
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

            var destinationAddress = GetStreamIp(stream);

            var messageBytes = _encoding.GetBytes(message);
            var lengthBytes = BitConverter.GetBytes(messageBytes.Length);

            var semaphore = GetSemaphore(client);

            var retryCount = 0;

            var retryLogger = _logger.ForContext("MessageText", message).ForContext("RetryCount", retryCount);

            while (retryCount < 3)
            {
                try
                {
                    await semaphore.WaitAsync(cancellationToken);

                    cancellationToken.ThrowIfCancellationRequested();

                    await stream.WriteAsync(lengthBytes, cancellationToken);
                    await stream.WriteAsync(messageBytes, cancellationToken);

                    retryLogger.Debug(DebugMessageTemplates.Messages.MessageSent, destinationAddress);

                    return;
                }
                catch (OperationCanceledException ex)
                {
                    retryLogger.Debug(ex, DebugMessageTemplates.Messages.MessageSendCancelled, destinationAddress);
                    return;
                }
                catch (IOException ex)
                {
                    if (ex.InnerException is SocketException socketEx && socketEx.SocketErrorCode == SocketError.ConnectionReset)
                    {
                        retryCount++;

                        retryLogger.Warning(ex, ErrorMessageTemplates.Messages.MessageSendWarning, destinationAddress);

                        continue;
                    }

                    retryLogger.Error(ex, ErrorMessageTemplates.Messages.MessageSendFailed, destinationAddress);

                    throw new MessageException(ex, "Failed to send message", destinationAddress, message);
                }
                catch (Exception ex)
                {
                    retryLogger.Error(ex, ErrorMessageTemplates.Messages.MessageSendFailed, destinationAddress);
                    throw new MessageException(ex, "Failed to send message", destinationAddress, message);
                }
                finally
                {
                    semaphore.Release();
                }
            }
        }

        public async Task<string> ReadMessageAsync(TcpClient client, CancellationToken cancellationToken = default)
        {
            var stream = client.GetStream();

            var remoteAddress = GetStreamIp(stream);
            var retryCount = 0;

            var retryLogger = _logger.ForContext("RetryCount", retryCount);

            while (retryCount < 3)
            {
                try
                {
                    var lengthBytes = new byte[4];
                    await stream.ReadAsync(lengthBytes, cancellationToken);

                    var messageLength = BitConverter.ToInt32(lengthBytes, 0);
                    var messageBytes = new byte[messageLength];
                    await stream.ReadAsync(messageBytes.AsMemory(0, messageLength), cancellationToken);

                    var message = _encoding.GetString(messageBytes);

                    retryLogger.ForContext("MessageText", message)
                        .Debug(DebugMessageTemplates.Messages.MessageRead, remoteAddress);

                    return message;
                }
                catch (OperationCanceledException)
                {
                    retryLogger.Debug(DebugMessageTemplates.Messages.MessageReadCancalled, remoteAddress);
                    throw;
                }
                catch (IOException ex)
                {
                    if (ex.InnerException is SocketException socketEx && socketEx.SocketErrorCode == SocketError.ConnectionReset)
                    {
                        retryCount++;

                        retryLogger.Warning(ex, ErrorMessageTemplates.Messages.MessageReadWarning, remoteAddress);

                        continue;
                    }

                    retryLogger.Error(ex, ErrorMessageTemplates.Messages.MessageReadFailed, remoteAddress);

                    throw new MessageException(ex, "Failed to read incoming message", remoteAddress);
                }
                catch (Exception ex)
                {
                    retryLogger.Error(ex, ErrorMessageTemplates.Messages.MessageReadFailed, remoteAddress);
                    throw new MessageException(ex, "Failed to read incoming message", remoteAddress);
                }
            }

            throw new MessageException("Retry limit reached while trying to read incoming message", remoteAddress);
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

        private static string GetStreamIp(NetworkStream stream)
        {

            return ((IPEndPoint?) stream.Socket.RemoteEndPoint)?.Address?.ToString()?.Replace("::ffff:", "") ?? "N/A";
        }

        public void DisplayConsoleMessage(string message)
        {
            Console.SetCursorPosition(0, Console.CursorTop);

            Console.Write("> ");
            WriteColoredMessage(message, ConsoleColor.Blue);
            Console.Write("> ");
        }

        private void WriteColoredMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"{message}\n");
            Console.ResetColor();
        }
    }
}