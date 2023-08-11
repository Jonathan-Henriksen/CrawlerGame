using NeuralJourney.Core.Models.Commands;

namespace NeuralJourney.Core.Interfaces.Commands
{
    public interface ICommandFactory
    {
        ICommand CreateCommand(CommandKey commandKey, string[]? parameters);
    }
}