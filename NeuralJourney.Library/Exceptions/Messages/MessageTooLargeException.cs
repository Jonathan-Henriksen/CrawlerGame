using NeuralJourney.Library.Constants;
using NeuralJourney.Library.Exceptions.Messages.Base;

namespace NeuralJourney.Library.Exceptions.Messages
{

    [Serializable]
    public class MessageTooLargeException : MessageSizeException
    {
        public MessageTooLargeException() { }
        public MessageTooLargeException(string message) : base(message) { }
        public MessageTooLargeException(string message, Exception inner) : base(message, inner) { }
        protected MessageTooLargeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public MessageTooLargeException(string messageText, int characterLimit) :
            base(string.Format(ErrorMessages.Messages.MessageTooLarge, characterLimit), messageText, characterLimit)
        { }
    }
}