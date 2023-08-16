using System.Runtime.Serialization;

namespace NeuralJourney.Core.Enums.Commands
{
    public enum CommandIdentifierEnum
    {
        [EnumMember(Value = "N/A")]
        NotAvailable,
        CheckMap,
        Move,
    }
}