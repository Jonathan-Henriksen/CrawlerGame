namespace NeuralJourney.Core.Constants.Messages
{
    public static class PlayerMessageTemplates
    {
        public static class Command
        {
            public const string InvalidCommand = "The command '{CommandIdentifier}' is not recognized. Please check the command and try again.";

            public const string InvalidParameter = "The value '{ParameterValue}' is not valid for paramter {ParameterName} the command '{CommandIdentifier}'. Expected: {ExpectedValue}'.";

            public const string MissingParameter = "The parameter '{ParameterName}' is required for the command '{CommandIdentifier}'. Please provide the necessary information and try again.";

            public const string InvalidCompletionText = "An error occurred while processing your request. Please try again.";
        }

        public static class Message
        {
            public const string TooLarge = "The length of the provided message({MessageSize} characters) exceeds the allowed length of {MessageSizeLimit}. Please shorten your message and try again.";

            public const string TooSmall = "The provided message is too short. Please provide more details and try again.";

            public const string InvalidFormat = "The format of your message is not valid. Please check and resend.";
        }

        public static class Map
        {
            public const string LimitReached = "You have reached the limit in this direction. Please explore a different path.";
        }

        public static class Network
        {
            public const string ConnectionLost = "Connection lost. Please check your network and try reconnecting.";

            public const string ConnectionError = "A network error occurred. Please try again later.";

            public const string RetryLimitReached = "The program reached the try limit of {RetryLimit} while processing {ProcessName}}";
        }
    }
}