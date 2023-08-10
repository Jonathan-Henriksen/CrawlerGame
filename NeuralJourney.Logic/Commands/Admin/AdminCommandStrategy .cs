using NeuralJourney.Library.Constants;
using NeuralJourney.Library.Exceptions.Commands;
using NeuralJourney.Library.Models.Commands;
using Serilog;

namespace NeuralJourney.Logic.Commands.Admin
{
    public class AdminCommandStrategy : ICommandStrategy
    {
        private readonly ICommandFactory _commandFactory;
        private readonly ILogger _logger;

        public AdminCommandStrategy(ICommandFactory commandFactory, ILogger logger)
        {
            _commandFactory = commandFactory;
            _logger = logger;
        }

        public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                context.CommandKey = new CommandKey(context.CommandType, default);

                context.Command = _commandFactory.CreateCommand(context.CommandKey.Value, context.Params);

                if (context.Command is null)
                    return;

                await context.Command.ExecuteAsync();

                _logger.Information(InfoMessageTemplates.ExecutedCommand, context.CommandKey?.Type, context.CommandKey?.Identifier);
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
        }
    }
}