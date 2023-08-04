using NeuralJourney.Library.Models.CommandInfo;
using NeuralJourney.Logic.Commands.Admin.Base;
using NeuralJourney.Logic.Commands.Players.Base;
using NeuralJourney.Logic.Dispatchers.Interfaces;
using NeuralJourney.Logic.Factories.Interfaces;

namespace NeuralJourney.Logic.Dispatchers
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly ICommandFactory<AdminCommand, AdminCommandInfo> _adminCommandFactory;
        private readonly ICommandFactory<PlayerCommand, PlayerCommandInfo> _playerCommandFactory;

        public CommandDispatcher(ICommandFactory<AdminCommand, AdminCommandInfo> adminCommandFactory, ICommandFactory<PlayerCommand, PlayerCommandInfo> playerCommandFactory)
        {
            _adminCommandFactory = adminCommandFactory;
            _playerCommandFactory = playerCommandFactory;
        }

        public async Task DispatchAdminCommandAsync(AdminCommandInfo commandInfo)
        {
            var command = _adminCommandFactory.GetCommand(commandInfo);

            if (command is null)
                return;

            await command.ExecuteAsync();
        }

        public async Task DispatchPlayerCommandAsync(PlayerCommandInfo commandInfo)
        {
            var command = _playerCommandFactory.GetCommand(commandInfo);

            if (command is null)
                return;

            await command.ExecuteAsync();
        }
    }
}