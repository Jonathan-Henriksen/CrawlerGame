namespace NeuralJourney.Core.Exceptions
{

    [Serializable]
    public class CommandException : Exception
    {
        private string? _playerMessage;

        public string PlayerMessage
        {
            get { return _playerMessage ?? string.Empty; }
            set { _playerMessage = value; }
        }

        public CommandException() { }
        public CommandException(string message) : base(message) { }
        public CommandException(string message, Exception inner) : base(message, inner) { }
        protected CommandException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public CommandException(string message, string playerMessage) : base(message)
        {
            _playerMessage = playerMessage;
        }

        public CommandException(string message, string playerMessage, Exception inner) : base(message, inner)
        {
            PlayerMessage = playerMessage;
        }
    }
}