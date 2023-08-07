namespace NeuralJourney.Logic.CommandStrategies.Interfaces
{
    public interface IAdminCommandStrategy
    {
        Task ExecuteAsync(string adminInput);
    }
}