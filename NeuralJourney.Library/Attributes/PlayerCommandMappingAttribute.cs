using NeuralJourney.Library.Enums;

namespace NeuralJourney.Library.Attributes
{
    public class PlayerCommandMappingAttribute : Attribute
    {
        public PlayerCommandEnum Command { get; }

        public PlayerCommandMappingAttribute(PlayerCommandEnum enumValue)
        {
            Command = enumValue;
        }
    }
}