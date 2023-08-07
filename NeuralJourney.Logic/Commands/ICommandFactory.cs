using NeuralJourney.Library.Models.CommandContext;

namespace NeuralJourney.Logic.Commands
{
    public interface ICommandFactory
    {
        CommandBase CreateCommand(CommandContext commandContext);
    }
}