namespace NeuralJourney.Core.Options
{
    public sealed class SerilogOptions
    {
        public string? LogLevel { get; set; }

        public string? LogFilePath { get; set; }

        public string? ConsoleOutputTemplate { get; set; }

        public string? FileOutputTemplate { get; set; }
    }
}