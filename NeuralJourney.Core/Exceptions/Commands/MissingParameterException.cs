using NeuralJourney.Core.Constants.Messages;
using NeuralJourney.Core.Enums.Commands;

namespace NeuralJourney.Core.Exceptions.Commands
{
    [Serializable]
    public class MissingParameterException : GameException
    {
        public readonly CommandIdentifierEnum Command;

        public MissingParameterException(CommandIdentifierEnum command, string paramName) :
            base(PlayerMessageTemplates.Command.InvalidParameter, ErrorMessageTemplates.Command.MissingParameter, paramName, command)
        {
            Command = command;
        }
    }
}