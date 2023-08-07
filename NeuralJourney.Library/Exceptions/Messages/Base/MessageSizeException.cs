namespace NeuralJourney.Library.Exceptions.Messages.Base
{

    [Serializable]
    public class MessageSizeException : MessageException
    {
        public int MessageSize { get; set; }

        public int MessageSizeLimit { get; set; }

        public MessageSizeException() { }
        public MessageSizeException(string message) : base(message) { }
        public MessageSizeException(string message, Exception inner) : base(message, inner) { }
        protected MessageSizeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public MessageSizeException(string message, string messageText, int messageSizeLimit) :
            base(message, messageText)
        {
            MessageSize = messageText.Length;
            MessageSizeLimit = messageSizeLimit;
        }
    }
}