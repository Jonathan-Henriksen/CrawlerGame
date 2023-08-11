using NeuralJourney.Core.Commands.Admin;
using NeuralJourney.Core.Commands.Players;
using NeuralJourney.Core.Enums.Commands;
using NeuralJourney.Core.Interfaces.Commands;

namespace NeuralJourney.Core.Commands
{
    public class CommandStrategyFactory : ICommandStrategyFactory
    {
        private readonly IEnumerable<ICommandStrategy> _commandStrategies;

        public CommandStrategyFactory(IEnumerable<ICommandStrategy> commandStrategies)
        {
            _commandStrategies = commandStrategies;
        }

        public ICommandStrategy CreateCommandStrategy(CommandTypeEnum type)
        {
            return type switch
            {
                CommandTypeEnum.Admin => _commandStrategies.First(s => s.GetType() == typeof(AdminCommandStrategy)),
                CommandTypeEnum.Player => _commandStrategies.First(s => s.GetType() == typeof(PlayerStrategy)),
                _ => throw new ArgumentOutOfRangeException(nameof(type)),
            };
        }
    }
}