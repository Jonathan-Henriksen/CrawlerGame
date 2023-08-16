using NeuralJourney.Core.Constants.Messages;

namespace NeuralJourney.Core.Exceptions
{
    [Serializable]
    public class CommandMappingException : CommandException
    {
        private const string _defaultPlayerMessage = PlayerMessageTemplates.Command.NoMatch;

        public CommandMappingException() { }
        public CommandMappingException(string message) : base(message, _defaultPlayerMessage) { }

        public CommandMappingException(string message, string? playerMessage = null) : base(message, playerMessage ?? _defaultPlayerMessage) { }

        public CommandMappingException(Exception inner, string message, string? playerMessage = null) : base(message, playerMessage ?? _defaultPlayerMessage, inner) { }

        protected CommandMappingException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}