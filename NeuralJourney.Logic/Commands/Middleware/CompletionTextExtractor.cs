using NeuralJourney.Library.Enums.Commands;
using NeuralJourney.Library.Exceptions.Commands;
using NeuralJourney.Library.Models.Commands;
using System.Text.RegularExpressions;

namespace NeuralJourney.Logic.Commands.Middleware
{
    public class CompletionTextExtractor : ICommandMiddleware
    {
        public async Task InvokeAsync(CommandContext context, Func<Task> next, CancellationToken cancellationToken = default)
        {
            var completionText = context.CompletionText ?? throw new InvalidCompletionTextException(context.CompletionText, "Completion text was blank");

            var regexPattern = @"^(?<commandIdentifier>\w+)\((?<params>[^\)]+)\)\|(?<successMessage>.+?)$";
            var match = Regex.Match(completionText, regexPattern);

            if (!match.Success)
                throw new InvalidCompletionTextException(context.CompletionText, "Invalid command format.");

            var commandIdentifierText = match.Groups["commandIdentifier"].Value;
            if (!Enum.TryParse(commandIdentifierText, true, out CommandIdentifierEnum commandIdentifier))
                throw new InvalidCompletionTextException(completionText, $"Could not parse the command '{commandIdentifierText}' to {nameof(CommandIdentifierEnum)}");

            context.CommandKey = new CommandKey(CommandTypeEnum.Player, commandIdentifier);

            context.Params = match.Groups["params"].Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            context.ExecutionMessage = match.Groups["successMessage"].Value;

            if (string.IsNullOrEmpty(context.ExecutionMessage))
                throw new InvalidCompletionTextException(context.CompletionText, "Execution message was blank");

            await next();
        }
    }
}