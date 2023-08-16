namespace NeuralJourney.Core.Constants
{
    public static class MessageLogTemplates
    {
        public static class Debug
        {
            public const string MessageRead = "Received message from {Address}";

            public const string MessageReadCancelled = "Cancelled reading message from {Address}";

            public const string MessageSent = "Sent message to {Address}";



            public const string MessageSendCancelled = "Cancelled sending message to {Address}";

            public const string FailedToReadConsoleInput = "Failed to read console input";
        }

        public static class Warning
        {
            public const string StreamWriteFailed = "Failed to write to stream {Address}";

            public const string StreamReadFailed = "Failed to read from stream {Address}";
        }

        public static class Error
        {
            public const string MessageReadFailed = "Failed to read message from {Address}";

            public const string MessageSendFailed = "Failed to send message to {Address}";



            public const string UnexpectedErrorWhileHandlingNetworkInput = "Unexpected error while handling network input";
            public const string ThereWasAnErrorProcessingInput = "There was an error processing the input. Please try again";
        }
    }

}
