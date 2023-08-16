namespace NeuralJourney.Core.Exceptions
{

    [Serializable]
    public class MessageException : Exception
    {
        public string? MessageText { get; set; }

        public string? Address { get; set; }

        public MessageException() { }
        public MessageException(string message) : base(message) { }
        public MessageException(string message, Exception inner) : base(message, inner) { }

        public MessageException(string message, string? address, string? messageText = null) : this(message)
        {
            Address = address;
            MessageText = messageText;
        }

        public MessageException(Exception ex, string message, string? address, string? messageText = null) : this(message, ex)
        {
            Address = address;
            MessageText = messageText;
        }

        protected MessageException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}