namespace NeuralJourney.Logic.Options
{
    public sealed class OpenAIOptions
    {
        public string ApiKey { get; set; } = string.Empty;

        public string? Model { get; set; }

        public string? StopSequence { get; set; }

        public int? MaxTokens { get; set; }

        public double? Temperature { get; set; }
    }
}