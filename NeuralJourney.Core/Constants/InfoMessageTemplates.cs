namespace NeuralJourney.Core.Constants
{
    public static class InfoMessageTemplates
    {
        public const string ClientConnected = "Client connected from {0}";

        public const string ClientDisconnected = "Client disconnected from {0}";

        public const string ExecutedCommand = "Executed {0} command {1}";

        public const string GameStarted = "The game engine was started";

        public const string GameStopped = "The game engine was stopped";

        public const string PlayerAdded = "Added player '{0}' to the game";

        public const string PlayerRemoved = "Removed player '{0}' from the game";

        public const string ServerStarted = "The server was started and is now listening for incoming connections";

        public const string ServerStopped = "The server was stopped and is no longer listening for incomming connections";
    }
}