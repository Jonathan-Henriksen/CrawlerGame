using NeuralJourney.Core.Enums.Commands;
using NeuralJourney.Core.Models.Commands;
using Serilog;

namespace NeuralJourney.Core.Extensions
{
    public static class SerilogExtensions
    {
        public static object ToSimplified(this CommandContext context)
        {
            var commandKey = new { Type = context.CommandKey.Type, Identifier = context.CommandKey.Identifier.WithCorrectNAValue() };
            return new { Command = commandKey, Params = context.Params, UserInput = context.RawInput, CompletionText = context.CompletionText };
        }

        private static string WithCorrectNAValue(this CommandIdentifierEnum value)
        {
            if (value == CommandIdentifierEnum.NotAvailable)
                return "N/A";
            return value.ToString();
        }
    }
}