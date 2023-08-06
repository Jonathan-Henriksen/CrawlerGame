using NeuralJourney.Library.Exceptions.Messages.Base;

namespace NeuralJourney.Library.Exceptions.Messages
{

    [Serializable]
    public class MessageTooSmallException : MessageSizeException
    {
        public MessageTooSmallException() { }
        public MessageTooSmallException(string message) : base(message) { }
        public MessageTooSmallException(string message, Exception inner) : base(message, inner) { }
        protected MessageTooSmallException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public MessageTooSmallException(string messageText, int characterLimit) :
            base(string.Format("The message \'{0}\' is too small. Messages must be larger than \'{1}\' characters", messageText, characterLimit), messageText, characterLimit)
        { }
    }
}