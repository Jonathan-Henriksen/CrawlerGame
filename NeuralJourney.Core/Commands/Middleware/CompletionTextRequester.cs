using NeuralJourney.Core.Exceptions.Commands;
using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Interfaces.Services;

namespace NeuralJourney.Core.Commands.Middleware
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
            context.CompletionText = await _openAIService.GetCommandCompletionTextAsync(context.RawInput);

            if (string.IsNullOrEmpty(context.CompletionText))
                throw new InvalidCompletionTextException(context.CompletionText, "Completion text was empty");

            await next();
        }
    }
}