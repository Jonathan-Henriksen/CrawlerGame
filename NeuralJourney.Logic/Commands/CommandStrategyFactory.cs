using NeuralJourney.Library.Enums.Commands;
using NeuralJourney.Logic.Commands.Admin;
using NeuralJourney.Logic.Commands.Players;

namespace NeuralJourney.Logic.Commands
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
                CommandTypeEnum.Player => _commandStrategies.First(s => s.GetType() == typeof(PlayerCommandStrategy)),
                _ => throw new ArgumentOutOfRangeException(nameof(type)),
            };
        }
    }
}