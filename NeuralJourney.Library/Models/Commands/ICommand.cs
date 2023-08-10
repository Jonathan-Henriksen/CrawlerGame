namespace NeuralJourney.Library.Models.Commands
{
    public interface ICommand
    {
        public Task<CommandResult> ExecuteAsync();
    }
}
