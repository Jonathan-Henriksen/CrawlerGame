using NeuralJourney.Core.Constants;
using NeuralJourney.Core.Enums.Commands;

namespace NeuralJourney.Core.Exceptions.Commands
{
    [Serializable]
    public class InvalidCompletionTextException : GameException
    {
        public readonly CommandIdentifierEnum Command;
        public readonly string? CompletionText;
        public readonly string Reason;

        public InvalidCompletionTextException(string? completionText, string reason) :
            base(PlayerMessageTemplates.Command.InvalidCompletionText, ErrorMessageTemplates.Command.InvalidCompletionText, completionText, reason)
        {
            CompletionText = completionText;
            Reason = reason;
        }
    }
}