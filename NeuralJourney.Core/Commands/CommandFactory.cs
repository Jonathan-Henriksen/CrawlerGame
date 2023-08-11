using NeuralJourney.Core.Exceptions.Commands;
using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Models.Commands;

namespace NeuralJourney.Core.Commands
{
    public class CommandFactory : ICommandFactory
    {
        public ICommand CreateCommand(CommandKey key, string[]? parameters)
        {
            var commandType = CommandRegistry.GetCommandType(key);

            var command = (ICommand?) Activator.CreateInstance(commandType, parameters);

            return command ?? throw new InvalidCommandException(command: key.Identifier, $"Failed to create an instance of '{commandType.FullName}'");
        }
    }
}