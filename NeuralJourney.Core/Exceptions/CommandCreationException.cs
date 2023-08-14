namespace NeuralJourney.Core.Exceptions
{

    [Serializable]
    public class CommandCreationException : Exception
    {
        public CommandCreationException() { }
        public CommandCreationException(string message) : base(message) { }
        public CommandCreationException(string message, Exception inner) : base(message, inner) { }
        protected CommandCreationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}