using NeuralJourney.Library.Attributes;
using NeuralJourney.Library.Enums.Commands;
using NeuralJourney.Library.Exceptions.Commands;
using NeuralJourney.Library.Models.CommandContext;
using System.Reflection;

namespace NeuralJourney.Logic.Commands
{
    public class CommandFactory : ICommandFactory
    {
        private readonly Dictionary<CommandKey, Type> _commandMappings = new();

        public CommandFactory()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    var identifierAttribute = type.GetCustomAttribute<CommandAttribute>();
                    var commandTypeAttribute = type.GetCustomAttribute<CommandTypeAttribute>();

                    if (identifierAttribute != null && commandTypeAttribute != null)
                    {
                        var key = new CommandKey(commandTypeAttribute.CommandType, identifierAttribute.Identifier);
                        _commandMappings[key] = type;
                    }
                }
            }
        }

        public CommandBase CreateCommand(CommandContext commandContext)
        {
            var key = new CommandKey(commandContext.CommandType, commandContext.CommandIdentifier);

            if (!_commandMappings.TryGetValue(key, out var commandType))
                throw new InvalidCommandException($"No command registered for type: {commandType} and identifier: {commandContext.CommandIdentifier}");

            var command = (CommandBase?) Activator.CreateInstance(commandType, commandContext);
            return command ?? throw new CommandMappingException($"Failed to create an instance of '{commandType.FullName}'");
        }
    }
}