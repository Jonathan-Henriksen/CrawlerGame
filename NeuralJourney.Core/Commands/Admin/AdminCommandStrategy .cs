using NeuralJourney.Core.Constants;
using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Models.LogProperties;
using Serilog;

namespace NeuralJourney.Core.Commands.Admin
{
    public class AdminCommandStrategy : ICommandStrategy
    {
        private readonly ICommandFactory _commandFactory;
        private readonly ILogger _logger;

        public AdminCommandStrategy(ICommandFactory commandFactory, ILogger logger)
        {
            _commandFactory = commandFactory;
            _logger = logger.ForContext<AdminCommandStrategy>();
        }

        public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                context.Command = _commandFactory.CreateCommand(context);

                if (context.Command is null)
                    return;

                await context.Command.ExecuteAsync();

                _logger.Information(CommandLogMessages.Info.ExecutedCommand, context.CommandKey.Identifier);
            }
            catch (Exception)
            {
                // TODO: Implement
            }
        }
    }
}