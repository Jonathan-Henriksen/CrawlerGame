using NeuralJourney.Library.Attributes;
using NeuralJourney.Library.Enums;
using NeuralJourney.Library.Models.CommandInfo;
using NeuralJourney.Logic.Commands.Players;
using NeuralJourney.Logic.Commands.Players.Base;
using NeuralJourney.Logic.Factories.Interfaces;
using NeuralJourney.Logic.Options;
using System.Reflection;

namespace NeuralJourney.Logic.Factories
{
    public class PlayerCommandFactory : ICommandFactory<PlayerCommand, PlayerCommandInfo>
    {
        private readonly Dictionary<PlayerCommandEnum, Type> _commands = new();

        private readonly GameOptions _gameOptions;

        public PlayerCommandFactory(GameOptions gameOptions)
        {
            _gameOptions = gameOptions;

            var assembly = Assembly.GetAssembly(this.GetType());

            var commandTypes = assembly?
                .GetExportedTypes()
                .Where(type => typeof(PlayerCommand).IsAssignableFrom(type) && type != typeof(PlayerCommand) && type.CustomAttributes.Any(a => a.AttributeType == typeof(PlayerCommandMappingAttribute)))
                .Select(t => (Type: t, CommandAttribute: t.GetCustomAttribute<PlayerCommandMappingAttribute>()));

            foreach (var commandType in commandTypes ?? Enumerable.Empty<(Type, PlayerCommandMappingAttribute)>())
            {
                if (commandType.CommandAttribute is null)
                    continue;

                _commands.Add(commandType.CommandAttribute.Command, commandType.Type);
            }
        }

        public PlayerCommand GetCommand(PlayerCommandInfo commandInfo)
        {
            if (_commands.TryGetValue(commandInfo.PlayerCommand, out var commandType))
            {
                return (PlayerCommand) (Activator.CreateInstance(commandType, commandInfo, _gameOptions) ?? new UnknownPlayerCommand(commandInfo));
            }

            return new UnknownPlayerCommand(commandInfo);
        }
    }
}