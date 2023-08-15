using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Models.Commands;

namespace NeuralJourney.Core.Commands
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
            var strategy = _commandStrategyFactory.CreateCommandStrategy(context.CommandKey.Type)
                ?? throw new InvalidOperationException("Failed to dispatch command. Reason: No strategy was available for the provided command type");

            _ = strategy.ExecuteAsync(context);
        }
    }
}