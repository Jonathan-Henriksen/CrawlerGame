using NeuralJourney.Core.Exceptions;
using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Models.Commands;
using System.Reflection;

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

                return command ?? throw new CommandMappingException("Failed to create an instance of the coomand");
            }
            catch (MissingMethodException ex)
            {
                throw new CommandMappingException(ex, "No matching constructor found");
            }
            catch (TargetException ex)
            {
                throw new CommandMappingException(ex, "Constructor threw an exception");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}