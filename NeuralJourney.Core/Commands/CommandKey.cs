using NeuralJourney.Core.Enums.Commands;

namespace NeuralJourney.Core.Commands
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

        public override bool Equals(object? obj)
        {
            return obj is CommandKey key &&
                   Type == key.Type &&
                   Identifier == key.Identifier;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Identifier);
        }
    }
}