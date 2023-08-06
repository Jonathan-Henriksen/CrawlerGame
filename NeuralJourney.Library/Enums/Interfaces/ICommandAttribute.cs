namespace NeuralJourney.Library.Enums.Interfaces
{
    public interface ICommandAttribute<TCommandEnumType>
    {
        public TCommandEnumType Command { get; }
    }
}