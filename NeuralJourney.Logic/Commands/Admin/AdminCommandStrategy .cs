using NeuralJourney.Library.Enums.Commands;
using NeuralJourney.Library.Exceptions.Commands;

namespace NeuralJourney.Logic.Commands.Admin
{
    public class AdminCommandStrategy : IAdminCommandStrategy
    {
        private readonly ICommandFactory _commandFactory;

        public AdminCommandStrategy(ICommandFactory commandFactory)
        {
            _commandFactory = commandFactory;
        }

        public async Task ExecuteAsync(string adminInput)
        {
            try
            {
                var commandContext = new CommandContext(CommandIdentifierEnum.Unknown, Array.Empty<string>(), adminInput);

                var command = _commandFactory.CreateCommand(commandContext);

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