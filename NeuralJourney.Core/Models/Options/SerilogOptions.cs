namespace NeuralJourney.Core.Models.Options
{
    public sealed class SerilogOptions
    {
        public string? LogLevel { get; set; }

        public string? SeqUrl { get; set; }

        public string? ConsoleOutputTemplate { get; set; }
    }
}