using NeuralJourney.Library.Models.CommandInfo.Base;
using NeuralJourney.Logic.Commands;

namespace NeuralJourney.Logic.Factories.Interfaces
{
    public interface ICommandFactory<TCommandType, TCommandEnumType>
        where TCommandType : CommandBase
        where TCommandEnumType : Enum
    {
        TCommandType CreateCommand(ICommandInfo<TCommandEnumType> commandInfo);
    }
}