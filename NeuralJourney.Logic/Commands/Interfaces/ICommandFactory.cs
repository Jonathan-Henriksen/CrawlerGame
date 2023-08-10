using NeuralJourney.Library.Models.Commands;

namespace NeuralJourney.Logic.Commands.Interfaces
{
    public interface ICommandFactory
    {
        ICommand CreateCommand(CommandKey commandKey, string[]? parameters);
    }
}