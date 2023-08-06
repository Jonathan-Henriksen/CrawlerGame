using NeuralJourney.Library.Exceptions.Messages.Base;

namespace NeuralJourney.Library.Exceptions.Messages
{

    [Serializable]
    public class InvalidMessageFormatException : MessageException
    {
        public InvalidMessageFormatException() { }
        public InvalidMessageFormatException(string message) : base(message) { }
        public InvalidMessageFormatException(string message, Exception inner) : base(message, inner) { }
        protected InvalidMessageFormatException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public InvalidMessageFormatException(string messageText, string reason) :
            base(string.Format("The message \'{0}\' was formatted incorrectly.\nReason: \'{1}\'", messageText, reason), messageText)
        { }
    }
}