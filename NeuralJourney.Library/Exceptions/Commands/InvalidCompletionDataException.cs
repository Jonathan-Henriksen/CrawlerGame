using NeuralJourney.Library.Exceptions.Commands.Base;

namespace NeuralJourney.Library.Exceptions.Commands
{

    [Serializable]
    public class InvalidCompletionDataException : CommandMappingException
    {
        public InvalidCompletionDataException() { }
        public InvalidCompletionDataException(string message) : base(message) { }
        public InvalidCompletionDataException(string message, Exception inner) : base(message, inner) { }
        protected InvalidCompletionDataException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public InvalidCompletionDataException(string? completion, string reason) :
            base(string.Format("The completion text \'{0}\' was invalid. Reason: {1}", completion, reason))
        { }
    }
}