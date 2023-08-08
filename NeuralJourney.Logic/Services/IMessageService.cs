namespace NeuralJourney.Logic.Services
{
    public interface IMessageService
    {
        Task SendMessageAsync(Stream stream, string message);

        Task<string> ReadMessageAsync(Stream stream);

        Task SendCloseConnectionAsync(Stream stream);

        bool IsCloseConnectionMessage(string message);
    }
}