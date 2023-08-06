namespace NeuralJourney.Library.Models.CommandInfo.Base
{
    public abstract class CommandInfoBase<TCommandEnumType> : ICommandInfo<TCommandEnumType>
    {
        public CommandInfoBase(TCommandEnumType commandEnum, string[]? @params, string successMessage)
        {
            CommandEnum = commandEnum;
            Params = @params;
            SuccessMessage = successMessage;
        }

        public TCommandEnumType CommandEnum { get; }

        public string[]? Params { get; }

        public string SuccessMessage { get; }
    }
}