namespace NeuralJourney.Library.Enums.Commands.Interfaces
{
    public interface ICommandAttribute<TCommandEnumType>
    {
        public TCommandEnumType Command { get; }
    }
}