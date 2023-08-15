﻿namespace NeuralJourney.Core.Exceptions
{

    [Serializable]
    public class CommandException : Exception
    {
        public readonly string? PlayerMessage;

        public CommandException() { }
        public CommandException(string message) : base(message) { }
        public CommandException(string message, Exception inner) : base(message, inner) { }
        protected CommandException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public CommandException(string message, string playerMessage) : base(message)
        {
            PlayerMessage = playerMessage;
        }

        public CommandException(string message, string playerMessage, Exception inner) : base(message, inner)
        {
            PlayerMessage = playerMessage;
        }
    }
}