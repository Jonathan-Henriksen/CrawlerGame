using NeuralJourney.Core.Constants;
using NeuralJourney.Core.Exceptions;
using NeuralJourney.Core.Extensions;
using NeuralJourney.Core.Interfaces.Services;
using NeuralJourney.Core.Models.LogProperties;
using Serilog;
using Serilog.Context;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace NeuralJourney.Infrastructure.Services
{
    public partial class MessageService : IMessageService
    {
        private readonly Dictionary<TcpClient, SemaphoreSlim> _streamSemaphores = new();
        private readonly Encoding _encoding = Encoding.UTF8;
        private readonly ILogger _logger;

        public const string CloseConnectionMessage = "__CLOSE_CONNECTION__";
        public const string NameVerificationMessage = "_NAME_{0}_NAME__ID__{1}__ID_";

        public MessageService(ILogger logger)
        {
            _logger = logger.ForContext<MessageService>();
        }

        public async Task SendMessageAsync(TcpClient client, string message, CancellationToken cancellationToken = default)
        {
            var stream = client.GetStream();

            var messageContext = new MessageContext(message, 0, client.GetRemoteIp());

            var messageBytes = _encoding.GetBytes(message);
            var lengthBytes = BitConverter.GetBytes(messageBytes.Length);

            var semaphore = GetSemaphore(client);

            var messageLogger = _logger.ForContext(nameof(MessageContext), messageContext, true);

            while (messageContext.RetryCount < 3)
            {
                try
                {
                    await semaphore.WaitAsync(cancellationToken);

                    cancellationToken.ThrowIfCancellationRequested();

                    await stream.WriteAsync(lengthBytes, cancellationToken);
                    await stream.WriteAsync(messageBytes, cancellationToken);

                    messageLogger.Debug(MessageLogMessages.Debug.Sent, messageContext.IpAddress);

                    return;
                }
                catch (OperationCanceledException ex)
                {
                    if (stream.DataAvailable)
                        messageLogger.Debug(ex, MessageLogMessages.Debug.SendCancelled, messageContext.IpAddress);

                    return;
                }
                catch (IOException ex)
                {
                    if (ex.InnerException is SocketException socketEx && socketEx.SocketErrorCode == SocketError.ConnectionReset)
                    {
                        messageContext.RetryCount++;

                        messageLogger.Warning(ex, MessageLogMessages.Warning.StreamWriteFailed, messageContext.IpAddress);

                        continue;
                    }

                    messageLogger.Error(ex, MessageLogMessages.Error.SendFailed, messageContext.IpAddress);

                    throw new MessageException(ex, string.Format(MessageLogMessages.Error.SendFailed, messageContext.IpAddress), messageContext);
                }
                catch (Exception ex)
                {
                    messageLogger.Error(ex, MessageLogMessages.Error.SendFailed, messageContext.IpAddress);
                    throw new MessageException(ex, string.Format(MessageLogMessages.Error.SendFailed, messageContext.IpAddress), messageContext);
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

            var messageContext = new MessageContext("N/A", 0, client.GetRemoteIp());

            using (LogContext.PushProperty(nameof(MessageContext), messageContext, true))
            {
                while (messageContext.RetryCount < 3)
                {
                    try
                    {
                        var lengthBytes = new byte[4];
                        await stream.ReadAsync(lengthBytes, cancellationToken);

                        var messageLength = BitConverter.ToInt32(lengthBytes, 0);
                        var messageBytes = new byte[messageLength];
                        await stream.ReadAsync(messageBytes.AsMemory(0, messageLength), cancellationToken);

                        messageContext.MessageText = _encoding.GetString(messageBytes);

                        _logger.Debug(MessageLogMessages.Debug.Read, messageContext.IpAddress);

                        return messageContext.MessageText;
                    }
                    catch (OperationCanceledException)
                    {
                        if (stream.DataAvailable)
                            _logger.Debug(MessageLogMessages.Debug.ReadCancelled, messageContext.IpAddress);

                        throw;
                    }
                    catch (IOException ex) when (ex.InnerException is SocketException socketEx)
                    {
                        if (socketEx.SocketErrorCode == SocketError.ConnectionReset)
                        {
                            messageContext.RetryCount++;

                            _logger.Warning(ex, MessageLogMessages.Warning.StreamReadFailed, messageContext.IpAddress);

                            continue;
                        }

                        // Handle clienet being closed before token is cancelled
                        if (socketEx.SocketErrorCode == SocketError.OperationAborted)
                            throw new OperationCanceledException();

                        _logger.Error(ex, MessageLogMessages.Error.ReadFailed, messageContext.IpAddress);

                        throw new MessageException(ex, string.Format(MessageLogMessages.Error.ReadFailed, messageContext.IpAddress), messageContext);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, MessageLogMessages.Error.ReadFailed, messageContext.IpAddress);
                        throw new MessageException(ex, string.Format(MessageLogMessages.Error.ReadFailed, messageContext.IpAddress), messageContext);
                    }
                }
            }

            throw new MessageException(MessageLogMessages.Error.ReadRetryLimitReach, messageContext);
        }

        public async Task SendCloseConnectionAsync(TcpClient client, CancellationToken cancellationToken = default)
        {
            await SendMessageAsync(client, CloseConnectionMessage, cancellationToken);
        }

        public bool IsCloseConnectionMessage(string message)
        {
            return message == CloseConnectionMessage;
        }

        public async Task SendHandshake(TcpClient client, string name, Guid id, CancellationToken cancellationToken = default)
        {
            await SendMessageAsync(client, string.Format(NameVerificationMessage, name, id), cancellationToken);
        }

        public bool IsHandshake(string message, out string? name, out Guid? id)
        {
            name = null;
            id = null;

            var match = ValidateNameVerificationMessage().Match(message);

            if (!match.Success)
                return false;

            name = match.Groups[1].Value;

            if (!Guid.TryParse(match.Groups[2].Value, out var guid))
                return false;

            id = guid;
            return true;
        }

        public void DisplayConsoleMessage(string message)
        {
            Console.SetCursorPosition(0, Console.CursorTop);

            Console.Write("> ");
            WriteColoredMessage(message, ConsoleColor.Blue);
            Console.Write("> ");
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

        private static void WriteColoredMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"{message}\n");
            Console.ResetColor();
        }

        [GeneratedRegex(@"_NAME_(.*?)_NAME__ID__([a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12})__ID_")]
        private static partial Regex ValidateNameVerificationMessage();
    }
}