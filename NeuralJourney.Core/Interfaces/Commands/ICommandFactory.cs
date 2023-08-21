using NeuralJourney.Core.Models.LogProperties;

namespace NeuralJourney.Core.Interfaces.Commands
{
    public interface ICommandFactory
    {
        ICommand CreateCommand(CommandContext context);
    }
}