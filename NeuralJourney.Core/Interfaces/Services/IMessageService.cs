using System.Net.Sockets;

namespace NeuralJourney.Core.Interfaces.Services
{
    public interface IMessageService
    {
        Task SendMessageAsync(TcpClient stream, string message, CancellationToken cancellationToken = default);

        Task<string> ReadMessageAsync(TcpClient stream, CancellationToken cancellationToken = default);

        Task SendCloseConnectionAsync(TcpClient stream, CancellationToken cancellationToken = default);

        bool IsCloseConnectionMessage(string message);

        void DisplayConsoleMessage(string message);
    }
}