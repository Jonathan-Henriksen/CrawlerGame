namespace NeuralJourney.Core.Exceptions
{

    [Serializable]
    public class CommandExecutionException : Exception
    {
        public CommandExecutionException() { }
        public CommandExecutionException(string message) : base(message) { }
        public CommandExecutionException(string message, Exception inner) : base(message, inner) { }
        protected CommandExecutionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
