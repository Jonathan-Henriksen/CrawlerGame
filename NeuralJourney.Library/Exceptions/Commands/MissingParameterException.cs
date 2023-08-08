using NeuralJourney.Library.Constants;
using NeuralJourney.Library.Enums.Commands;

namespace NeuralJourney.Library.Exceptions.Commands
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