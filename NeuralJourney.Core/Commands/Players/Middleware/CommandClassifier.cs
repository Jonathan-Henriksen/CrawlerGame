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
            var commandIdentifier = await _openAIService.GetCommandClassificationAsync(context.InputText);
            context.CommandKey.Identifier = commandIdentifier;

            _logger.Debug("Input was classified as the command {CommandIdentifier}", commandIdentifier);

            await next();
        }
    }
}