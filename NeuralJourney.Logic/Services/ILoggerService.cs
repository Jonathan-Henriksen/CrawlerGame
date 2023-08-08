namespace NeuralJourney.Logic.Services
{
    public interface ILoggerService
    {
        void LogInfo(string message);

        void LogException(Exception ex, string message);
    }
}
