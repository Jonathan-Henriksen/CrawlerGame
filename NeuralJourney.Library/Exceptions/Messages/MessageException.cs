namespace NeuralJourney.Library.Exceptions.Messages
{

    [Serializable]
    public class MessageException : Exception
    {
        public string? MessageText { get; set; }

        public MessageException() { }
        public MessageException(string message) : base(message) { }
        public MessageException(string message, Exception inner) : base(message, inner) { }
        protected MessageException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }


        public MessageException(string message, string? messageText) : base(message)
        {
            MessageText = messageText;
        }
    }
}