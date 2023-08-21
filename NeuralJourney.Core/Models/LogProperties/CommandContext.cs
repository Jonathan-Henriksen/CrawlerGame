using NeuralJourney.Core.Enums.Commands;
using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Models.Commands;
using NeuralJourney.Core.Models.World;

namespace NeuralJourney.Core.Models.LogProperties
{
    public class CommandContext
    {
        public string InputText { get; }
        public Player? Player { get; }

        public ICommand? Command { get; set; }
        public CommandKey CommandKey { get; set; }
        public string? CompletionText { get; set; }
        public string? ExecutionMessage { get; set; }
        public string[] Params { get; set; }
        public CommandResult? Result { get; set; }

        public CommandContext(string rawInput, Player? player = default)
        {
            InputText = rawInput;
            Player = player;
            Params = Array.Empty<string>();
            var commandType = player is not null ? CommandTypeEnum.Player : CommandTypeEnum.Admin;
            CommandKey = new CommandKey(commandType);
        }
    }
}