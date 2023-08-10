using NeuralJourney.Library.Models.Commands;

namespace NeuralJourney.Logic.Commands
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly ICommandStrategyFactory _commandStrategyFactory;

        public CommandDispatcher(ICommandStrategyFactory commandStrategyFactory)
        {
            _commandStrategyFactory = commandStrategyFactory;
        }

        public void DispatchCommand(CommandContext context)
        {
            var strategy = _commandStrategyFactory.CreateCommandStrategy(context.CommandType)
                ?? throw new InvalidOperationException("Failed to dispatch command. Reason: No strategy was availabl for the provided command type");

            _ = strategy.ExecuteAsync(context);
        }
    }
}