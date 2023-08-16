namespace NeuralJourney.Core.Constants
{
    public static class ServerLogTemplates
    {
        public static class Debug
        {
            public const string DispatchedPlayerCommand = "Dispatching a command for {PlayerName}";
        }

        public static class Info
        {
            public const string PlayerAdded = "Added player {PlayerName} to the game";

            public const string PlayerRemoved = "Player {PlayerName} disconnected";

            public const string ServerStarted = "The server was started";

            public const string ServerStopped = "The server was stopped";
        }

        public static class Error
        {
            public const string PlayerInputFailed = "Failed to handle player input";

            public const string UnexpectedError = "The server encountered an unexpected error";
        }
    }
}