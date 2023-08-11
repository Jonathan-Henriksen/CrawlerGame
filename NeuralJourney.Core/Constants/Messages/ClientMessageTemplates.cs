namespace NeuralJourney.Core.Constants.Messages
{
    public static class ClientMessageTemplates
    {
        public const string ClientNotInitialized = "Operation failed: The client have not been initialized.";

        public const string ConnectionClosed = "Server closed the connection unexpectedly.";

        public const string ConnectionEstablished = "Connection established with the server.";

        public const string ConnectionFailed = "Failed to connect to the server.";

        public const string ConnectionInitialize = "Connecting to the server.";

        public const string StopGame = "Stopping the game.";
    }
}