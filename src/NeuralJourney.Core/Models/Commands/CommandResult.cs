namespace NeuralJourney.Core.Models.Commands
{
    public readonly struct CommandResult
    {
        public CommandResult(bool success, string playerMessage, string? additionalPlayerMessage = null)
        {
            Success = success;
            AdditionalMessage = additionalPlayerMessage;
            PlayerMessage = playerMessage;
        }

        public bool Success { get; }

        public string PlayerMessage { get; }

        public string? AdditionalMessage { get; }
    }
}