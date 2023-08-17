using System.Net.Sockets;

namespace NeuralJourney.Core.Interfaces.Services
{
    public interface IMessageService
    {
        Task SendMessageAsync(TcpClient client, string message, CancellationToken cancellationToken = default);

        Task<string> ReadMessageAsync(TcpClient client, CancellationToken cancellationToken = default);

        Task SendCloseConnectionAsync(TcpClient client, CancellationToken cancellationToken = default);

        bool IsCloseConnectionMessage(string message);

        void DisplayConsoleMessage(string message);
    }
}