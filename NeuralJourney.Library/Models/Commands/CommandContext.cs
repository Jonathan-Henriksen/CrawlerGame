using NeuralJourney.Library.Enums.Commands;
using NeuralJourney.Library.Models.World;

namespace NeuralJourney.Library.Models.Commands
{
    public readonly struct CommandContext
    {
        public CommandContext(CommandIdentifierEnum commandIdentifier, string[]? @params, string executionMessage, Player? player = null)
        {
            CommandIdentifier = commandIdentifier;
            Params = @params;
            ExecutionMessage = executionMessage;
            Player = player;
        }

        public CommandTypeEnum CommandType { get; }

        public CommandIdentifierEnum CommandIdentifier { get; }

        public string[]? Params { get; }

        public string ExecutionMessage { get; }

        public Player? Player { get; }
    }
}