using NeuralJourney.Core.Enums.Commands;

namespace NeuralJourney.Core.Interfaces.Commands
{
    public interface ICommandStrategyFactory
    {
        ICommandStrategy CreateCommandStrategy(CommandTypeEnum type);
    }
}