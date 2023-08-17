namespace NeuralJourney.Core.Constants
{
    public static class NetworkLogMessages
    {
        public static class Debug
        {
            public const string ClientConnected = "Client connected from {Address}";

            public const string TcpListenerStarted = "TCP Listener was started and is now listening for incoming connections";
        }

        public static class Warning
        {
            public const string OpenAIRequestTimeout = "Network error while requesting OpenAI completion";

            public const string SocketFailureRetry = "Error while listening for incoming connections {RetryCount}/{RetryLimit}";
        }

        public static class Error
        {
            public const string SocketFailure = "Failed to accept incoming connections. Retry limit reached";            

            public const string UnexpectedError = "Unexpected network error";
        }
    }
}
