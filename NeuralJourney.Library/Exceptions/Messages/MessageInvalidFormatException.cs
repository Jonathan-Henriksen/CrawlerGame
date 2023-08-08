using NeuralJourney.Library.Constants;

namespace NeuralJourney.Library.Exceptions.Messages
{

    [Serializable]
    public class MessageInvalidFormatException : MessageException
    {
        public MessageInvalidFormatException() { }
        public MessageInvalidFormatException(string message) : base(message) { }
        public MessageInvalidFormatException(string message, Exception inner) : base(message, inner) { }
        protected MessageInvalidFormatException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public MessageInvalidFormatException(string messageText, string reason) :
            base(string.Format(ErrorMessages.Messages.MessageFormat, reason), messageText)
        { }
    }
}