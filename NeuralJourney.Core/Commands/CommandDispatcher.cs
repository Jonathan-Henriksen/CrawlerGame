using NeuralJourney.Core.Constants;
using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Interfaces.Services;
using NeuralJourney.Core.Models.Commands;
using Serilog;

namespace NeuralJourney.Core.Commands
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly ICommandStrategyFactory _commandStrategyFactory;
        private readonly IMessageService _messageService;
        private readonly ILogger _logger;

        public CommandDispatcher(ICommandStrategyFactory commandStrategyFactory, IMessageService messageService, ILogger logger)
        {
            _commandStrategyFactory = commandStrategyFactory;
            _messageService = messageService;
            _logger = logger;
        }

        public void DispatchCommand(CommandContext context)
        {
            try
            {
                var strategy = _commandStrategyFactory.CreateCommandStrategy(context.CommandKey.Type);

                if (strategy is null)
                {
                    _logger.ForContext("Reason", "No strategy was found for the command type")
                        .Error(CommandLogMessages.Error.CommandDispatchFailed, context.CommandKey.Type);

                    if (context.Player is not null)
                        _messageService.SendMessageAsync(context.Player.GetClient(), PlayerMessages.Command.NoMatch);

                    return;
                }

                if (context.Player is not null)
                    _logger.Debug(ServerLogMessages.Debug.DispatchedPlayerCommand, context.Player.Name);

                _ = strategy.ExecuteAsync(context);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, CommandLogMessages.Error.CommandDispatchFailed, context.CommandKey.Type);

                if (context.Player is not null)
                    _messageService.SendMessageAsync(context.Player.GetClient(), PlayerMessages.SomethingWentWrong);
            }
        }
    }
}