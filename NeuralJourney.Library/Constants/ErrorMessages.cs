namespace NeuralJourney.Library.Constants
{
    public static class ErrorMessages
    {
        public static class Commands
        {
            public const string CommandMappingError = "The command '{0}' does not map to any known actions. Please try one of the other available commands '{1}'";

            public const string InvalidCommand = "The command '{0}' is invalid. Reason: {1}";

            public const string MissingParameter = "The command '{0}' requires the parameter '{1}'. Please provide the necessary parameters and try again.";

            public const string InvalidParameter = "The parameter '{0}' for the command '{1}' is invalid. Value: {2}. Expected format: {3}.";

            public const string InvalidCompletionData = "The completion text'{0}' returned from OpenAI was invalid. Reason: {1}";
        }

        public static class Messages
        {
            public const string MessageTooLarge = "The provided message exceeds the allowed size limit of {0} characeters. Please reduce the message size and try again.";

            public const string MessageTooSmall = "The provided message is below the minimum size requirement of {0} characeters. Please provide a valid message and try again.";

            public const string MessageFormat = "The provided message was formatted incorrectly. Reason: {0}";
        }

        public static class PlayerActions
        {
            public const string MapLimitReached = "You cannot move any further {0].";
        }
    }
}