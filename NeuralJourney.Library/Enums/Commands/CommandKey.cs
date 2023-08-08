namespace NeuralJourney.Library.Enums.Commands
{
    public readonly struct CommandKey
    {
        public CommandTypeEnum CommandType { get; }
        public CommandIdentifierEnum CommandIdentifier { get; }

        public CommandKey(CommandTypeEnum commandType, CommandIdentifierEnum commandIdentifier)
        {
            CommandType = commandType;
            CommandIdentifier = commandIdentifier;
        }

        public override bool Equals(object? obj)
        {
            return obj is CommandKey key &&
                   CommandType == key.CommandType &&
                   CommandIdentifier == key.CommandIdentifier;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CommandType, CommandIdentifier);
        }
    }
}