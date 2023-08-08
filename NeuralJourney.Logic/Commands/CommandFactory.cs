using NeuralJourney.Library.Exceptions.Commands;
using NeuralJourney.Library.Models.Commands;

namespace NeuralJourney.Logic.Commands
{
    public class CommandFactory : ICommandFactory
    {
        public CommandBase CreateCommand(CommandContext commandContext)
        {
            var commandType = CommandRegistry.GetCommandType(commandContext);

            var command = (CommandBase?) Activator.CreateInstance(commandType, commandContext);
            if (command == null)
            {
                throw new CommandMappingException($"Failed to create an instance of '{commandType.FullName}'");
            }

            return command;
        }
    }
}