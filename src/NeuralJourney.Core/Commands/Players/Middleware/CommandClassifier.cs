using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Interfaces.Services;
using NeuralJourney.Core.Models.LogProperties;
using Serilog;

namespace NeuralJourney.Core.Commands.Players.Middleware
{
    public class CommandClassifier : ICommandMiddleware
    {
        private readonly IOpenAIService _openAIService;
        private readonly ILogger _logger;

        public CommandClassifier(IOpenAIService openAIService, ILogger logger)
        {
            _openAIService = openAIService;
            _logger = logger;
        }

        public async Task InvokeAsync(CommandContext context, Func<Task> next, CancellationToken cancellationToken = default)
        {
            if ((context.Player?.HasIncompleteCommand ?? false) && (context.Player?.PreviousCommands.TryPeek(out var previousCommand) ?? false))
            {
                context.CommandKey.Identifier = previousCommand.CommandKey.Identifier;

                _logger.Debug("Continuing from previous incomplete command {CommandIdentifier}", context.CommandKey.Identifier);
            }
            else
            {
                context.CommandKey.Identifier = await _openAIService.GetCommandClassificationAsync(context.InputText);

                _logger.Debug("Input was classified as {CommandIdentifier} command", context.CommandKey.Identifier);
            }

            await next();
        }
    }
}