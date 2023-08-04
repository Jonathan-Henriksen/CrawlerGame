namespace NeuralJourney.Logic.Factories.Interfaces
{
    public interface ICommandFactory<TCommandType, TCommandInfoType>
    {
        internal TCommandType GetCommand(TCommandInfoType commandInfo);
    }
}