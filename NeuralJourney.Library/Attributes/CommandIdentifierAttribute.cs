using NeuralJourney.Library.Enums.Commands;

namespace NeuralJourney.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class CommandIdentifierAttribute : Attribute
    {
        public CommandIdentifierEnum CommandIdentifier { get; }

        public CommandIdentifierAttribute(CommandIdentifierEnum commandIdentifier)
        {
            CommandIdentifier = commandIdentifier;
        }
    }
}