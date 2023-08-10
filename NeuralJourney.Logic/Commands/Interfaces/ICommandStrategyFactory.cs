using NeuralJourney.Library.Enums.Commands;

namespace NeuralJourney.Logic.Commands.Interfaces
{
    public interface ICommandStrategyFactory
    {
        ICommandStrategy CreateCommandStrategy(CommandTypeEnum type);
    }
}