namespace NeuralJourney.Logic.Services.Interfaces
{
    public interface IMessageService
    {
        Task SendMessageAsync(Stream stream, string message, CancellationToken cancellationToken = default);

        Task<string> ReadMessageAsync(Stream stream, CancellationToken cancellationToken = default);

        Task SendCloseConnectionAsync(Stream stream, CancellationToken cancellationToken = default);

        bool IsCloseConnectionMessage(string message);
    }
}