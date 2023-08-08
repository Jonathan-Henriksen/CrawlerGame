using NeuralJourney.Library.Attributes;
using NeuralJourney.Library.Enums.Commands;
using NeuralJourney.Library.Exceptions.Commands;
using NeuralJourney.Library.Models.Commands;
using System.Reflection;

namespace NeuralJourney.Logic.Commands
{
    public static class CommandRegistry
    {
        private static readonly Dictionary<CommandKey, Type> _commandMappings = new();

        static CommandRegistry()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes().Where(t => t.CustomAttributes.Any(a => a.AttributeType == typeof(CommandAttribute))))
                {
                    var commandAttribute = type.GetCustomAttribute<CommandAttribute>();
                    if (commandAttribute != null)
                    {
                        var key = new CommandKey(commandAttribute.Type, commandAttribute.Identifier);
                        _commandMappings[key] = type;
                    }
                }
            }
        }

        public static Type GetCommandType(CommandContext commandContext)
        {
            var key = new CommandKey(commandContext.CommandType, commandContext.CommandIdentifier);
            if (!_commandMappings.TryGetValue(key, out var commandType))
            {
                throw new InvalidCommandException($"No command registered for type: {commandContext.CommandType} and identifier: {commandContext.CommandIdentifier}");
            }

            return commandType;
        }

        public static string GetAllPlayerCommands()
        {
            var playerCommands = _commandMappings
                .Where(kvp => kvp.Key.CommandType == CommandTypeEnum.Player)
                .Select(kvp => kvp.Key.CommandIdentifier.ToString())
                .ToList();

            return string.Join(", ", playerCommands);
        }
    }

}