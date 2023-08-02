using NeuralJourney.Library.Enums;
using NeuralJourney.Library.Models.World;
using System.Text.Json.Serialization;

namespace NeuralJourney.Library.Models.ChatGPT
{
    public class CommandInfo
    {
        [JsonConstructor]
        public CommandInfo(CommandEnum command, string[] @params, string successMessage)
        {
            Command = command;
            Params = @params;
            SuccessMessage = successMessage;
        }

        public CommandInfo(CommandEnum command, string successMessage)
        {
            Command = command;
            Params = Array.Empty<string>();
            SuccessMessage = successMessage;
        }

        public CommandEnum Command { get; set; }

        [JsonPropertyName("Param")]
        public string[] Params { get; set; }

        public string SuccessMessage { get; set; }

        public string? FailureMessage { get; set; }

        public Player? Player { get; set; }
    }
}