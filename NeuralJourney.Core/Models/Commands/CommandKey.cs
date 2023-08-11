using NeuralJourney.Core.Enums.Commands;

namespace NeuralJourney.Core.Models.Commands
{
    public readonly struct CommandKey
    {
        public CommandTypeEnum Type { get; }
        public CommandIdentifierEnum Identifier { get; }

        public CommandKey(CommandTypeEnum type, CommandIdentifierEnum identifier)
        {
            Type = type;
            Identifier = identifier;
        }
    }
}