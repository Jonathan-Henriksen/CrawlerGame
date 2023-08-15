using NeuralJourney.Core.Enums.Commands;
using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Models.World;

namespace NeuralJourney.Core.Models.Commands
{
    public class CommandContext
    {
        public CommandContext(string rawInput, Player? player = default)
        {
            RawInput = rawInput;
            Player = player;

            var commandType = player is not null ? CommandTypeEnum.Player : CommandTypeEnum.Admin;
            CommandKey = new CommandKey(commandType);
        }

        public readonly string RawInput;

        public readonly Player? Player;


        public ICommand? Command { get; set; }

        public CommandKey CommandKey { get; set; }

        public string? CompletionText { get; set; }

        public string? ExecutionMessage { get; set; }

        public string[]? Params { get; set; }

        public CommandResult? Result { get; set; }
    }
}