using NeuralJourney.Core.Constants;
using NeuralJourney.Core.Enums.Commands;

namespace NeuralJourney.Core.Exceptions.Commands
{
    [Serializable]
    public class InvalidParameterException : GameException
    {
        public readonly CommandIdentifierEnum Command;
        public readonly string ParameterName;
        public readonly object? ParameterValue;
        public readonly string ExpectedValue;

        public InvalidParameterException(CommandIdentifierEnum command, string paramName, object? paramValue, string expectedValue) :
            base(PlayerMessageTemplates.Command.InvalidParameter, ErrorMessageTemplates.Command.InvalidParameter, paramValue, paramName, command, expectedValue)
        {
            Command = command;
            ParameterName = paramName;
            ParameterValue = paramValue;
            ExpectedValue = expectedValue;
        }
    }
}