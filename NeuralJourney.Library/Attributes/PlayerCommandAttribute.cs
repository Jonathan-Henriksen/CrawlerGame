using NeuralJourney.Library.Enums;

namespace NeuralJourney.Library.Attributes
{
    public class PlayerCommandAttribute : Attribute
    {
        public PlayerCommandEnum Command { get; }

        public PlayerCommandAttribute(PlayerCommandEnum enumValue)
        {
            Command = enumValue;
        }
    }
}