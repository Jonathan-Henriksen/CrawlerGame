using NeuralJourney.Library.Exceptions.Commands.Base;

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

        public InvalidParameterException(string commandName, string paramName, object? paramValue) :
            base(string.Format("The value \'{0}\' is not valid for the parameter \'{1}\' om the command \'{2}\'", paramValue, paramName, commandName), commandName, paramName)
        { }
    }
}