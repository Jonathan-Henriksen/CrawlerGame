namespace NeuralJourney.Core.Constants
{
    public static class MessageLogMessages
    {
        public static class Debug
        {
            public const string Read = "Received message from {Address}";

            public const string ReadCancelled = "Cancelled reading message from {Address}";

            public const string Sent = "Sent message to {Address}";

            public const string SendCancelled = "Cancelled sending message to {Address}";

            public const string FailedToReadConsoleInput = "Failed to read console input";
        }

        public static class Warning
        {
            public const string StreamWriteFailed = "Failed to write to stream {Address}";

            public const string StreamReadFailed = "Failed to read from stream {Address}";
        }

        public static class Error
        {
            public const string ReadFailed = "Failed to read message from {Address}";

            public const string SendFailed = "Failed to send message to {Address}";

            public const string ReadRetryLimitReach = "Retry limit reached while reading";

            public const string UnexpectedErrorWhileHandlingNetworkInput = "Unexpected error while handling network input";
            public const string ThereWasAnErrorProcessingInput = "There was an error processing the input. Please try again";
        }
    }

}
