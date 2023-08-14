namespace NeuralJourney.Core.Constants.Messages
{
    public static class ErrorMessageTemplates
    {
        public static class Command
        {
            public const string InvalidCommand = "Invalid command '{CommandIdentifier}' received from player '{PlayerName}'";

            public const string InvalidParameter = "Invalid value '{ParameterValue}' in parameter '{ParameterName}' for command '{CommandIdentifier}. Expected: {ExpectedValue}'";

            public const string MissingParameter = "Missing required parameter '{ParameterName}' for command '{CommandIdentifier}'";

            public const string InvalidCompletionText = "Invalid completion text received: {CompletionText}. Reason: {Reason}";
        }

        public static class Message
        {
            public const string TooLarge = "Message size exceeds the character limit. Received size: {MessageSize}. Size Limit: {MessageSizeLimit}";

            public const string TooSmall = "Message size is below the minimum required size. Received size: {MessageSize}";

            public const string InvalidFormat = "Message received in invalid format: {MessageText}. Reason: {Reason}";
        }

        public static class Map
        {
            public const string LimitReached = "Map limit reached for player while heading {Direction}. Player Name: '{PlayerName}'";
        }

        public static class Network
        {
            public const string RetryConnection = "{Service } encountered an unexpected error.";

            public const string RetryLimitReached = "Retry limit reached({RetryLimit}) while processing {ProcessName}";
        }
    }
}