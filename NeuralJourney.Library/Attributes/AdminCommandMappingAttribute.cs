using NeuralJourney.Library.Enums;

namespace NeuralJourney.Library.Attributes
{
    public class AdminCommandMappingAttribute : Attribute
    {
        public AdminCommandEnum Command { get; }

        public AdminCommandMappingAttribute(AdminCommandEnum enumValue)
        {
            Command = enumValue;
        }
    }
}