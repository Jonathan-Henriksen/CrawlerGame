using NeuralJourney.Core.Enums.Commands;

namespace NeuralJourney.Core.Commands
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class CommandAttribute : Attribute
    {
        public CommandIdentifierEnum Identifier { get; }
        public CommandTypeEnum Type { get; }

        public CommandAttribute(CommandTypeEnum type, CommandIdentifierEnum identifier)
        {
            Identifier = identifier;
            Type = type;
        }
    }
}