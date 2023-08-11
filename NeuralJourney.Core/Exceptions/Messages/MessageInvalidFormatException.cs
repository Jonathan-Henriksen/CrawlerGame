using NeuralJourney.Core.Constants.Messages;

namespace NeuralJourney.Core.Exceptions.Messages
{
    [Serializable]
    public class MessageInvalidFormatException : GameException
    {
        public readonly string MessageText;

        public readonly string Reason;

        public MessageInvalidFormatException(string messageText, string reason) :
            base(PlayerMessageTemplates.Message.InvalidFormat, ErrorMessageTemplates.Message.InvalidFormat, messageText, reason)
        {
            MessageText = messageText;
            Reason = reason;
        }
    }
}