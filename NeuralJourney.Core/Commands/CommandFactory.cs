using NeuralJourney.Core.Exceptions;
using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Models.LogProperties;
using NeuralJourney.Core.Models.Options;
using System.Reflection;

namespace NeuralJourney.Core.Commands
{
    public class CommandFactory : ICommandFactory
    {
        private readonly GameOptions _gameOptions;

        public CommandFactory(GameOptions gameOptions)
        {
            _gameOptions = gameOptions;
        }

        public ICommand CreateCommand(CommandContext context)
        {
            try
            {
                var commandType = CommandRegistry.GetCommandType(context.CommandKey);
                var commandParams = new object?[] { context, _gameOptions };

                var command = (ICommand?) Activator.CreateInstance(commandType, commandParams);

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