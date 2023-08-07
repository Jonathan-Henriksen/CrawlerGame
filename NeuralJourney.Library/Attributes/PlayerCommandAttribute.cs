using NeuralJourney.Library.Enums.Commands;

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