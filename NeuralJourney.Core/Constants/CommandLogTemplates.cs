namespace NeuralJourney.Core.Constants
{
    public static class CommandLogTemplates
    {
        public static class Debug
        {
            public const string PlayerWasNull = "Player was null";
            public const string CommandKeyWasNull = "CommandKey was null";
            public const string CommandWasNull = "Command was null";
        }

        public static class Info
        {
            public const string ExecutedCommand = "Executed command {CommandIdentifier}";
        }

        public static class Warning
        {
            public const string FailedToMapInput = "Failed to map input to command";
        }

        public static class Error
        {
            public const string CommandDispatchFailed = "Failed to dispatch {CommandType} command";



            public const string FailedToCreateInstance = "Failed to create an instance of the command";
            public const string NoMatchingConstructor = "No matching constructor found";
            public const string ConstructorThrewException = "Constructor threw an exception";
            public const string NoStrategyFound = "No strategy was found for the command type";
            public const string SomethingWentWrong = "SomethingWentWrong";
            public const string ErrorWhileExecutingCommand = "Error while executing command";
        }
    }

}
