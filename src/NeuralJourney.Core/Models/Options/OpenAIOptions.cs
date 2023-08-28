namespace NeuralJourney.Core.Models.Options
{
    public sealed class OpenAIOptions
    {
        public string ApiKey { get; set; } = string.Empty;

        public string? ParameterExtractionModel { get; set; }

        public string? CommandClassificationModel { get; set; }

        public string? StopSequence { get; set; }

        public int? MaxTokens { get; set; }

        public double? Temperature { get; set; }
    }
}