using NeuralJourney.Library.Enums.Interfaces;
using NeuralJourney.Library.Exceptions.Commands;
using NeuralJourney.Library.Exceptions.Commands.Base;
using NeuralJourney.Library.Models.CommandInfo.Base;
using NeuralJourney.Logic.Commands;
using NeuralJourney.Logic.Factories.Interfaces;
using NeuralJourney.Logic.Options;
using System.Reflection;

namespace NeuralJourney.Logic.Factories
{
    public class CommandFactory<TCommandType, TCommandEnumType> : ICommandFactory<TCommandType, TCommandEnumType>
        where TCommandType : CommandBase
        where TCommandEnumType : Enum
    {
        protected readonly Dictionary<TCommandEnumType, Type> Commands = new();
        protected readonly GameOptions _gameOptions;

        public CommandFactory(GameOptions gameOptions)
        {
            _gameOptions = gameOptions;

            var assembly = Assembly.GetAssembly(GetType());

            var commandTypes = assembly?
                .GetExportedTypes()
                .Where(type => type.CustomAttributes.Any(a => a.AttributeType.GetInterfaces().Contains(typeof(ICommandAttribute<TCommandEnumType>))))
                .Select(t => (Type: t, CommandAttribute: (ICommandAttribute<TCommandEnumType>?) t.GetCustomAttributes().FirstOrDefault(a => a is ICommandAttribute<TCommandEnumType>)));

            foreach (var commandType in commandTypes ?? Enumerable.Empty<(Type, ICommandAttribute<TCommandEnumType>)>())
            {
                if (commandType.CommandAttribute is null)
                    continue;

                Commands.Add(commandType.CommandAttribute.Command, commandType.Type);
            }
        }

        public TCommandType CreateCommand(ICommandInfo<TCommandEnumType> commandInfo)
        {
            if (!Commands.TryGetValue(commandInfo.CommandEnum, out var commandType) || commandType is null)
                throw new InvalidCommandException($"{commandInfo.CommandEnum}", "Could not find any implementations of the command");

            var command = (TCommandType?) Activator.CreateInstance(commandType, commandInfo, _gameOptions);

            return command ?? throw new CommandMappingException($"{commandInfo.CommandEnum}", string.Format("Failed to create an instance of \'{0}\'", commandType.FullName));
        }

        public TCommandType GetCommand(CommandInfoBase<Enum> commandInfo)
        {
            throw new NotImplementedException();
        }
    }
}