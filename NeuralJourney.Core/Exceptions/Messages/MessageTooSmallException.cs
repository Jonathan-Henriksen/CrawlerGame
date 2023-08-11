using NeuralJourney.Core.Constants.Messages;

namespace NeuralJourney.Core.Exceptions.Messages
{
    [Serializable]
    public class MessageTooSmallException : GameException
    {
        public readonly string MessageText;

        public readonly int CharacterLimit;

        public MessageTooSmallException(string messageText, int characterLimit) :
            base(PlayerMessageTemplates.Message.TooSmall, ErrorMessageTemplates.Message.TooSmall, messageText.Length, characterLimit)
        {
            MessageText = messageText;
            CharacterLimit = characterLimit;
        }
    }
}