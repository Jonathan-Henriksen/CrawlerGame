using NeuralJourney.Library.Constants;
using NeuralJourney.Library.Enums.Commands;
using NeuralJourney.Library.Exceptions.Commands;
using NeuralJourney.Library.Models.Commands;
using Serilog;

namespace NeuralJourney.Logic.Commands.Admin
{
    public class AdminCommandStrategy : IAdminCommandStrategy
    {
        private readonly ICommandFactory _commandFactory;
        private readonly ILogger _logger;

        public AdminCommandStrategy(ICommandFactory commandFactory, ILogger logger)
        {
            _commandFactory = commandFactory;
            _logger = logger;
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

                _logger.Information(InfoMessageTemplates.ExecutedCommand, commandContext.CommandType, commandContext.CommandIdentifier);
            }
            catch (InvalidCommandException ex)
            {
                _logger.Error(ex, ex.Message);
            }
            catch (MissingParameterException ex)
            {
                _logger.Error(ex, ex.Message);
            }
            catch (InvalidParameterException ex)
            {
                _logger.Error(ex, ex.Message);
            }
            catch (CommandMappingException ex)
            {
                _logger.Error(ex, ex.Message);
            }
        }
    }
}