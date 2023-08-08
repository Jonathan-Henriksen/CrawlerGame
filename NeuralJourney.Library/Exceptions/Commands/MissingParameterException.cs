using NeuralJourney.Library.Constants;

namespace NeuralJourney.Library.Exceptions.Commands
{

    [Serializable]
    public class MissingParameterException : CommandParameterException
    {
        public MissingParameterException() { }
        public MissingParameterException(string message) : base(message) { }
        public MissingParameterException(string message, Exception inner) : base(message, inner) { }
        protected MissingParameterException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public MissingParameterException(string commandName, string paramName) :
            base(string.Format(ErrorMessages.Commands.MissingParameter, commandName, paramName), commandName, paramName)
        { }
    }
}