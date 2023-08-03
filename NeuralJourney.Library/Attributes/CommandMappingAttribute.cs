using NeuralJourney.Library.Enums;

namespace NeuralJourney.Library.Attributes
{
    public class CommandMappingAttribute : Attribute
    {
        public CommandEnum Command { get; }

        public CommandMappingAttribute(CommandEnum enumValue)
        {
            Command = enumValue;
        }
    }
}