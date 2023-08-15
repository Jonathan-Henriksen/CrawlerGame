namespace NeuralJourney.Core.Exceptions
{
    [Serializable]
    public class CommandExecutionException : CommandException
    {
        private const string _defaultPlayerMessage = "Failed to execute command";

        public CommandExecutionException() { }
        public CommandExecutionException(string message) : base(message, _defaultPlayerMessage) { }
        public CommandExecutionException(string message, Exception inner) : base(message, _defaultPlayerMessage, inner) { }

        public CommandExecutionException(string message, string? playerMessage = null) : base(message, playerMessage ?? _defaultPlayerMessage) { }

        public CommandExecutionException(Exception inner, string message, string? playerMessage = null) : base(message, playerMessage ?? _defaultPlayerMessage, inner) { }

        protected CommandExecutionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
