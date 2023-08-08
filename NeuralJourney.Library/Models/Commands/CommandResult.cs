namespace NeuralJourney.Library.Models.Commands
{
    public class CommandResult
    {
        public CommandResult() { }

        public CommandResult(string additionalMessage)
        {
            AdditionalMessage = additionalMessage;
        }

        public string? AdditionalMessage { get; set; }
    }
}
