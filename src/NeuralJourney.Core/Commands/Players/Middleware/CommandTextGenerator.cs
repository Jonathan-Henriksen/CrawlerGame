using NeuralJourney.Core.Exceptions;
using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Interfaces.Services;
using NeuralJourney.Core.Models.LogProperties;
using Serilog;

namespace NeuralJourney.Core.Commands.Players.Middleware
{
    public class CommandTextGenerator : ICommandMiddleware
    {
        private readonly IOpenAIService _openAIService;
        private readonly ILogger _logger;

        public CommandTextGenerator(IOpenAIService openAIService, ILogger logger)
        {
            _openAIService = openAIService;
            _logger = logger;
        }

        public async Task InvokeAsync(CommandContext context, Func<Task> next, CancellationToken cancellationToken = default)
        {
            if (!await _openAIService.SetCommandCompletionTextAsync(context) || string.IsNullOrEmpty(context.CompletionText))
                throw new CommandMappingException("Completion text was empty", "The game encountered an error with the OpenAI API. Please try agian");

            await next();
        }
    }
}