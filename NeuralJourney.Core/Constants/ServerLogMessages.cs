namespace NeuralJourney.Core.Constants
{
    public static class ServerLogMessages
    {
        public static class Info
        {
            public const string PlayerAdded = "Player {PlayerName} connected";

            public const string PlayerRemoved = "Player {PlayerName} disconnected";

            public const string ServerStarted = "The server was started";

            public const string ServerStopped = "The server was stopped";
        }

        public static class Warning
        {
            public const string PlayerLeftEarly = "Player disconnected before providing a name";
        }

        public static class Error
        {
            public const string PlayerInputFailed = "Failed to handle player input";

            public const string PlayerAddFailed = "Failed to add player";

            public const string UnexpectedError = "The server encountered an unexpected error";
        }
    }
}