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

        public override bool Equals(object? obj)
        {
            if (obj is CommandKey other)
            {
                return Type == other.Type && Identifier == other.Identifier;
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Type.GetHashCode();
                hash = hash * 23 + Identifier.GetHashCode();

                return hash;
            }
        }
    }
}