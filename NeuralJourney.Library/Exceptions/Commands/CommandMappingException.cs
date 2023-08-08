namespace NeuralJourney.Library.Exceptions.Commands
{
    [Serializable]
    public class CommandMappingException : Exception
    {
        public readonly string? CommandName;

        public CommandMappingException() { }
        public CommandMappingException(string message) : base(message) { }
        public CommandMappingException(string message, Exception inner) : base(message, inner) { }
        protected CommandMappingException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public CommandMappingException(string message, string commandName) : base(message)
        {
            CommandName = commandName;
        }
    }
}