namespace NeuralJourney.Core.Commands
{
    public readonly struct CommandResult
    {
        public CommandResult() { }

        public CommandResult(string? additionalMessage = null)
        {
            AdditionalMessage = additionalMessage;
        }

        public string? AdditionalMessage { get; }
    }
}