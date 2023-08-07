using NeuralJourney.Library.Enums.Commands;

namespace NeuralJourney.Library.Attributes
{
    public class AdminCommandAttribute : Attribute
    {
        public AdminCommandEnum Command { get; }

        public AdminCommandAttribute(AdminCommandEnum enumValue)
        {
            Command = enumValue;
        }
    }
}