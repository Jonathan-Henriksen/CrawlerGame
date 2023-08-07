using NeuralJourney.Library.Enums.Commands;

namespace NeuralJourney.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class CommandTypeAttribute : Attribute
    {
        public CommandTypeEnum CommandType { get; }

        public CommandTypeAttribute(CommandTypeEnum commandType)
        {
            CommandType = commandType;
        }
    }
}