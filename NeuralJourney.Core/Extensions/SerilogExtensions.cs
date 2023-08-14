using NeuralJourney.Core.Models.Commands;

namespace NeuralJourney.Core.Extensions
{
    public static class SerilogExtensions
    {
        public static object ToSimplified(this CommandContext context)
        {
            return new { Command = context.CommandKey?.Identifier, Params = context.Params, UserInput = context.RawInput, CompletionText = context.CompletionText };
        }
    }
}