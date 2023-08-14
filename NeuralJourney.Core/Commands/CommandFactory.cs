using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Models.Commands;

namespace NeuralJourney.Core.Commands
{
    public class CommandFactory : ICommandFactory
    {
        public ICommand CreateCommand(CommandKey key, string[]? parameters)
        {
            try
            {
                var commandType = CommandRegistry.GetCommandType(key);
                var command = (ICommand?) Activator.CreateInstance(commandType, parameters);

                return command ?? throw new InvalidOperationException("Failed to create an instance of the coomand");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}