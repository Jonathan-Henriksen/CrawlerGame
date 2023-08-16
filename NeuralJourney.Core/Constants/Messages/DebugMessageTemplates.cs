namespace NeuralJourney.Core.Constants.Messages
{
    public static class DebugMessageTemplates
    {
        public const string DispoedOfType = "Disposed of {Type}";

        public const string PlayerDispatchedCommand = "Dispatching a command for {PlayerName}";

        public static class Messages
        {
            public const string MessageSent = "Sent message to {Address}";

            public const string MessageRead = "Received message from {Address}";

            public const string MessageSendCancelled = "Cancelled sending message to {Address}";

            public const string MessageReadCancalled = "Cancelled reading message from {Address}";
        }
    }
}