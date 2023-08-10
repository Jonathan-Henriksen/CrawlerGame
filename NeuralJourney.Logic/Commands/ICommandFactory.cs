using NeuralJourney.Library.Models.Commands;

namespace NeuralJourney.Logic.Commands
{
    public interface ICommandFactory
    {
        ICommand CreateCommand(CommandKey commandKey, string[]? parameters);
    }
}