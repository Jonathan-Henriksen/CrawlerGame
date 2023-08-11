using NeuralJourney.Core.Models.Commands;

namespace NeuralJourney.Core.Interfaces.Commands
{
    public interface ICommand
    {
        public Task<CommandResult> ExecuteAsync();
    }
}
