using NeuralJourney.Core.Enums.Commands;

namespace NeuralJourney.Core.Models.Commands
{
    public class CommandKey
    {
        public CommandTypeEnum Type { get; }
        public CommandIdentifierEnum Identifier { get; set; }

        public CommandKey(CommandTypeEnum type, CommandIdentifierEnum identifier = default)
        {
            Type = type;
            Identifier = identifier;
        }
    }
}