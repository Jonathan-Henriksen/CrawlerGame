namespace NeuralJourney.Logic.Commands.Admin
{
    public interface IAdminCommandStrategy
    {
        Task ExecuteAsync(string adminInput);
    }
}