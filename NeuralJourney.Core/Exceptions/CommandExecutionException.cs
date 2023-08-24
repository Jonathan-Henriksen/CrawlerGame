namespace NeuralJourney.Core.Exceptions
{
    [Serializable]
    public class CommandExecutionException : CommandException
    {
        public CommandExecutionException(string message, string playerMessage) : base(message, playerMessage) { }

        protected CommandExecutionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}