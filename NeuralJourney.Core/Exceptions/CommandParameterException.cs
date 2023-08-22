namespace NeuralJourney.Core.Exceptions
{
    public class CommandParameterException : CommandException
    {
        public string ParameterName { get; set; }

        public CommandParameterException(string message, string parameterName) : base(message)
        {
            ParameterName = parameterName;
        }
    }
}