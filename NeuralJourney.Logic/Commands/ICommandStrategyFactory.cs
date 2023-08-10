using NeuralJourney.Library.Enums.Commands;

namespace NeuralJourney.Logic.Commands
{
    public interface ICommandStrategyFactory
    {
        ICommandStrategy CreateCommandStrategy(CommandTypeEnum type);
    }
}