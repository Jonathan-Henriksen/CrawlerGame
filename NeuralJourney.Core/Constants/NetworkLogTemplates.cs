namespace NeuralJourney.Core.Constants
{
    public static class NetworkLogTemplates
    {
        public static class Debug
        {
            public const string TcpListenerStarted = "TCP Listener was started and is now listening for incoming connections";
        }

        public static class Info
        {
            public const string ClientConnected = "Client connected from {Address}";

            public const string ClientDisconnected = "Client disconnected from {Address}";

            public const string ConnectionEstablished = "ConnectionEstablished";
        }

        public static class Warning
        {
            public const string SocketFailureRetry = "Error while listening for incoming connections {RetryCount}/{RetryLimit}";
        }

        public static class Error
        {
            public const string SocketFailure = "Failed to accept incoming connections. Retry limit reached";

            public const string UnexpectedError = "Unexpected network error";
        }
    }
}
