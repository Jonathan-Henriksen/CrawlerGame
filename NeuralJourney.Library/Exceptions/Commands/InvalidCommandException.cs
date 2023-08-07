using NeuralJourney.Library.Constants;
using NeuralJourney.Library.Exceptions.Commands.Base;

namespace NeuralJourney.Library.Exceptions.Commands
{
    [Serializable]
    public class InvalidCommandException : CommandMappingException
    {
        public InvalidCommandException() { }
        public InvalidCommandException(string message) : base(message) { }
        public InvalidCommandException(string message, Exception inner) : base(message, inner) { }
        protected InvalidCommandException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public InvalidCommandException(string command, string reason) :
            base(string.Format(ErrorMessages.Commands.InvalidCommand, command, reason), command)
        { }
    }
}