using NeuralJourney.Library.Enums;
using NeuralJourney.Library.Exceptions.Commands;
using NeuralJourney.Library.Exceptions.Commands.Base;
using NeuralJourney.Library.Exceptions.PlayerActions.Base;
using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Commands.Admin.Base;
using NeuralJourney.Logic.Commands.Players.Base;
using NeuralJourney.Logic.Dispatchers.Interfaces;
using NeuralJourney.Logic.Factories.Interfaces;

namespace NeuralJourney.Logic.Dispatchers
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly ICommandInfoFactory _commandInfoFactory;

        private readonly ICommandFactory<AdminCommand, AdminCommandEnum> _adminCommandFactory;
        private readonly ICommandFactory<PlayerCommand, PlayerCommandEnum> _playerCommandFactory;


        public CommandDispatcher(ICommandInfoFactory commandInfoFactory, ICommandFactory<AdminCommand, AdminCommandEnum> adminCommandFactory, ICommandFactory<PlayerCommand, PlayerCommandEnum> playerCommandFactory)
        {
            _commandInfoFactory = commandInfoFactory;

            _adminCommandFactory = adminCommandFactory;
            _playerCommandFactory = playerCommandFactory;
        }

        public async void DispatchAdminCommand(string input)
        {
            try
            {
                var commandInfo = await _commandInfoFactory.CreateAdminCommandInfoFromInputAsync(input);

                if (commandInfo is null)
                    return;

                var command = _adminCommandFactory.CreateCommand(commandInfo);

                if (command is null)
                    return;

                await command.ExecuteAsync();
            }
            catch (InvalidCommandException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (MissingParameterException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (InvalidParameterException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (CommandMappingException ex)
            {
                Console.WriteLine(ex.Message);
            }


        }

        public async void DispatchPlayerCommand((Player Player, string Input) inputContext)
        {
            try
            {
                var commandInfo = await _commandInfoFactory.CreatePlayerCommandInfoFromInputAsync(inputContext.Input, inputContext.Player);

                if (commandInfo is null)
                    return;

                var command = _playerCommandFactory.CreateCommand(commandInfo);

                if (command is null)
                    return;

                await command.ExecuteAsync();
            }
            catch (InvalidCommandException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (MissingParameterException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (InvalidParameterException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (CommandMappingException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (PlayerActionException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}