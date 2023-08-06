namespace NeuralJourney.Library.Models.CommandInfo.Base
{
    public interface ICommandInfo<TCommandEnumType>
    {
        TCommandEnumType CommandEnum { get; }

        string[]? Params { get; }

        string SuccessMessage { get; }
    }
}