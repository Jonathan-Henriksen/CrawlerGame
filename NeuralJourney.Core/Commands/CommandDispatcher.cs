using NeuralJourney.Core.Constants;
using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Interfaces.Services;
using NeuralJourney.Core.Models.Commands;
using Serilog;
using Serilog.Context;

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
            using (LogContext.PushProperty("CommandContext", context, true))
            {
                try
                {
                    var strategy = _commandStrategyFactory.CreateCommandStrategy(context.CommandKey.Type);

                    if (strategy is null)
                    {
                        _logger.ForContext("Reason", "No strategy was found for the command type")
                            .Error(CommandLogTemplates.Error.CommandDispatchFailed, context.CommandKey.Type);

                        if (context.Player is not null)
                            _messageService.SendMessageAsync(context.Player.GetClient(), PlayerMessageTemplates.Command.NoMatch);

                        return;
                    }

                    _ = strategy.ExecuteAsync(context);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, CommandLogTemplates.Error.CommandDispatchFailed, context.CommandKey.Type);

                    if (context.Player is not null)
                        _messageService.SendMessageAsync(context.Player.GetClient(), PlayerMessageTemplates.SomethingWentWrong);
                }
            }
        }
    }
}