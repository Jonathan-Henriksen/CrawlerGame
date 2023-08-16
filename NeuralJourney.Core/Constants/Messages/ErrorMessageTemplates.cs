namespace NeuralJourney.Core.Constants.Messages
{
    public static class ErrorMessageTemplates
    {
        public const string CommandDispatchFailed = "Failed to dispatch {CommandType} command";

        public static class Messages
        {
            public const string MessageSendFailed = "Failed to send message to {Address}";

            public const string MessageReadFailed = "Failed to read message from {Address}";
        }
    }
}