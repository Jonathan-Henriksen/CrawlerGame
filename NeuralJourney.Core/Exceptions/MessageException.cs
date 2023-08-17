using NeuralJourney.Core.Models.LogProperties;

namespace NeuralJourney.Core.Exceptions
{

    [Serializable]
    public class MessageException : Exception
    {
        public MessageContext? Context { get; set; }

        public MessageException() { }
        public MessageException(string message) : base(message) { }
        public MessageException(string message, Exception inner) : base(message, inner) { }

        public MessageException(string message, MessageContext context) : this(message)
        {
            Context = context;
        }

        public MessageException(Exception ex, string message, MessageContext context) : this(message, ex)
        {
            Context = context;
        }

        protected MessageException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}