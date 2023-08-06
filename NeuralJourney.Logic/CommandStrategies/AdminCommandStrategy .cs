using NeuralJourney.Library.Exceptions.Commands.Base;
using NeuralJourney.Library.Exceptions.Commands;
using NeuralJourney.Logic.CommandStrategies.Interfaces;
using NeuralJourney.Logic.Factories.Interfaces;
using NeuralJourney.Library.Enums;
using NeuralJourney.Logic.Commands.Admin.Base;
using NeuralJourney.Library.Models.CommandInfo;

namespace NeuralJourney.Logic.CommandStrategies
{
    public class AdminCommandStrategy : IAdminCommandStrategy
    {
        private readonly ICommandFactory<AdminCommand, AdminCommandEnum> _commandFactory;

        public AdminCommandStrategy(ICommandFactory<AdminCommand, AdminCommandEnum> commandFactory)
        {
            _commandFactory = commandFactory;
        }

        public async Task ExecuteAsync(string adminInput)
        {
            try
            {
                var commandInfo = new AdminCommandInfo(AdminCommandEnum.Announce, Array.Empty<string>(), adminInput);

                var command = _commandFactory.CreateCommand(commandInfo);

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
    }
}