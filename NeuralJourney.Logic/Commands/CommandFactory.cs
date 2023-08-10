using NeuralJourney.Library.Exceptions.Commands;
using NeuralJourney.Library.Models.Commands;
using NeuralJourney.Logic.Commands.Interfaces;

namespace NeuralJourney.Logic.Commands
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