﻿using NeuralJourney.Library.Constants;

namespace NeuralJourney.Library.Exceptions.Messages
{
    [Serializable]
    public class MessageTooSmallException : MessageSizeException
    {
        public MessageTooSmallException() { }
        public MessageTooSmallException(string message) : base(message) { }
        public MessageTooSmallException(string message, Exception inner) : base(message, inner) { }
        protected MessageTooSmallException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public MessageTooSmallException(string messageText, int messageSizeLimit) :
            base(string.Format(ErrorMessages.Messages.MessageTooSmall, messageSizeLimit), messageText, messageSizeLimit)
        { }
    }
}