using NeuralJourney.Core.Attributes;
using NeuralJourney.Core.Enums.Commands;
using NeuralJourney.Core.Exceptions;
using NeuralJourney.Core.Models.Commands;
using System.Reflection;

namespace NeuralJourney.Core.Commands
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

        public static Type GetCommandType(CommandKey key)
        {
            if (!_commandMappings.TryGetValue(key, out var commandType))
                throw new CommandMappingException("Command not found", "Could not match the input to any available commands. Please try rephrasing");

            return commandType;
        }

        public static string GetCommands(CommandTypeEnum type)
        {
            var commands = _commandMappings
                .Where(kvp => kvp.Key.Type == type)
                .Select(kvp => kvp.Key.Identifier.ToString())
                .ToList();

            return string.Join(",", commands);
        }
    }
}