namespace NeuralJourney.Library.Constants
{
    public static class InfoMessages
    {
        public static class System
        {
            public const string ClientConnected = "Client connected";

            public const string ClientDisconnected = "Client disconnected";

            public const string ClientError = "Error handling client";

            public const string StartingServer = "Starting server and waiting for clients";
        }

        public static class Failure
        {
            public const string CheckMap = "You do not have a map";

            public const string MovePlayer = "You cannot move any further {0}";

            public const string SomethingWentWrong = "Something went wrong";
        }
    }
}