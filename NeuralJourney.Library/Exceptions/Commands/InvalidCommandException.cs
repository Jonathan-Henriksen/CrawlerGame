using NeuralJourney.Library.Constants;
using NeuralJourney.Library.Enums.Commands;

namespace NeuralJourney.Library.Exceptions.Commands
{
    [Serializable]
    public class InvalidCommandException : GameException
    {
        public readonly CommandIdentifierEnum Command;

        public readonly string Reason;

        public InvalidCommandException(CommandIdentifierEnum command, string reason) :
            base(PlayerMessageTemplates.Command.InvalidCommand, ErrorMessageTemplates.Command.InvalidCommand, command, reason)
        {
            Command = command;
            Reason = reason;
        }
    }
}