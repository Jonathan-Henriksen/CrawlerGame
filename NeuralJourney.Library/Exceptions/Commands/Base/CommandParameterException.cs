namespace NeuralJourney.Library.Exceptions.Commands.Base
{

    [Serializable]
    public class CommandParameterException : CommandMappingException
    {
        public string? ParameterName { get; set; }

        public CommandParameterException() { }
        public CommandParameterException(string message) : base(message) { }
        public CommandParameterException(string message, Exception inner) : base(message, inner) { }
        protected CommandParameterException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public CommandParameterException(string message, string commandName, string parameterName) :
            base(message, commandName)
        {
            ParameterName = parameterName;
        }
    }
}