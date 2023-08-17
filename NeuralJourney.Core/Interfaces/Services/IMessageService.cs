using System.Net.Sockets;

namespace NeuralJourney.Core.Interfaces.Services
{
    public interface IMessageService
    {
        Task SendMessageAsync(TcpClient client, string message, CancellationToken cancellationToken = default);

        Task<string> ReadMessageAsync(TcpClient client, CancellationToken cancellationToken = default);

        Task SendCloseConnectionAsync(TcpClient client, CancellationToken cancellationToken = default);

        bool IsCloseConnectionMessage(string message);

        Task SendHandshake(TcpClient client, string name, Guid id, CancellationToken cancellationToken = default);

        bool IsHandshake(string message, out string? name, out Guid? id);

        void DisplayConsoleMessage(string message);
    }
}