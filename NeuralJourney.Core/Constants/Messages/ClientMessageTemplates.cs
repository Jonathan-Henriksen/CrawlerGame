namespace NeuralJourney.Core.Constants.Messages
{
    public static class ClientMessageTemplates
    {
        public const string ClientNotInitialized = "Could not start the game. Reason: The client have not been initialized";

        public const string ConnectionClosed = "Server closed the connection";

        public const string ConnectionEstablished = "Connection established with the server";

        public const string ConnectionFailed = "Failed to connect to the server. Reason: {Reason}";

        public const string ConnectionInitialize = "Connecting to the server...";

        public const string StartingGame = "Starting the game...";

        public const string StoppingGame = "Stopping the game...";
    }
}