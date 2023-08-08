using NeuralJourney.Library.Models.Commands;

namespace NeuralJourney.Logic.Commands
{
    public interface ICommandFactory
    {
        CommandBase CreateCommand(CommandContext commandContext);
    }
}