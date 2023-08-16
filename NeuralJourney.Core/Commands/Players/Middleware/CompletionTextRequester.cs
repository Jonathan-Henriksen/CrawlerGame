using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Interfaces.Services;
using NeuralJourney.Core.Models.Commands;

namespace NeuralJourney.Core.Commands.Players.Middleware
{
    public class CompletionTextRequester : ICommandMiddleware
    {
        private readonly IOpenAIService _openAIService;

        public CompletionTextRequester(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        public async Task InvokeAsync(CommandContext context, Func<Task> next, CancellationToken cancellationToken = default)
        {
            context.CompletionText = await _openAIService.GetCommandCompletionTextAsync(context);

            await next();
        }
    }
}