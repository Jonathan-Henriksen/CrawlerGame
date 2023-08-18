namespace NeuralJourney.Core.Constants
{
    public static class CommandLogMessages
    {
        public static class Debug
        {
            public const string CompletionTextRequested = "Requesting OpenAI Completion for input '{InputText}'";

            public const string CompletionTextReceived = "Received completion from OpenAI '{CompletionText}'";

            public const string DispatchedPlayerCommand = "Dispatched command for {PlayerName}";
        }

        public static class Info
        {
            public const string ExecutedCommand = "Executed command {CommandIdentifier}";
        }

        public static class Error
        {
            public const string CommandDispatchFailed = "Failed to dispatch {CommandType} command";

            public const string CompletionTextRequstFailed = "Unexpected error while requestion OpenAI completion";
        }
    }

}
