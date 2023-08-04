using NeuralJourney.Library.Attributes;
using NeuralJourney.Library.Enums;
using NeuralJourney.Library.Models.CommandInfo;
using NeuralJourney.Logic.Commands.Admin;
using NeuralJourney.Logic.Commands.Admin.Base;
using NeuralJourney.Logic.Factories.Interfaces;
using NeuralJourney.Logic.Options;
using System.Reflection;

namespace NeuralJourney.Logic.Factories
{
    public class AdminCommandFactory : ICommandFactory<AdminCommand, AdminCommandInfo>
    {
        private readonly Dictionary<AdminCommandEnum, Type> _commands = new();

        private readonly GameOptions _gameOptions;

        public AdminCommandFactory(GameOptions gameOptions)
        {
            _gameOptions = gameOptions;

            var assembly = Assembly.GetAssembly(this.GetType());

            var commandTypes = assembly?
                .GetExportedTypes()
                .Where(type => typeof(AdminCommand).IsAssignableFrom(type) && type != typeof(AdminCommand) && type.CustomAttributes.Any(a => a.AttributeType == typeof(AdminCommandMappingAttribute)))
                .Select(t => (Type: t, CommandAttribute: t.GetCustomAttribute<AdminCommandMappingAttribute>()));

            foreach (var adminCommandType in commandTypes ?? Enumerable.Empty<(Type, AdminCommandMappingAttribute)>())
            {
                if (adminCommandType.CommandAttribute is null)
                    continue;

                _commands.Add(adminCommandType.CommandAttribute.Command, adminCommandType.Type);
            }
        }

        public AdminCommand GetCommand(AdminCommandInfo commandInfo)
        {
            if (_commands.TryGetValue(commandInfo.AdminCommand, out var commandType))
            {
                return (AdminCommand) (Activator.CreateInstance(commandType, commandInfo, _gameOptions) ?? new UnknownAdminCommand(commandInfo));
            }

            return new UnknownAdminCommand(commandInfo);
        }
    }
}