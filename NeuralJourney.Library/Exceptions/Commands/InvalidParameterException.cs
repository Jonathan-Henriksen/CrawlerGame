using NeuralJourney.Library.Constants;

namespace NeuralJourney.Library.Exceptions.Commands
{

    [Serializable]
    public class InvalidParameterException : CommandParameterException
    {
        public string? ParamName { get; set; }

        public InvalidParameterException() { }
        public InvalidParameterException(string message) : base(message) { }
        public InvalidParameterException(string message, Exception inner) : base(message, inner) { }
        protected InvalidParameterException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public InvalidParameterException(string commandName, string paramName, object? paramValue, string expectedValue) :
            base(string.Format(ErrorMessages.Commands.InvalidParameter, paramName, commandName, paramValue, expectedValue), commandName, paramName)
        { }
    }
}