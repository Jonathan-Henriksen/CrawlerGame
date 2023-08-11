using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Interfaces.Services;

namespace NeuralJourney.Core.Commands.Middleware
{
    public class ResultProcessor : ICommandMiddleware
    {
        private readonly IMessageService _messageService;

        public ResultProcessor(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task InvokeAsync(CommandContext context, Func<Task> next, CancellationToken cancellationToken = default)
        {
            var result = context.Result ?? throw new InvalidOperationException("Could not process result. Reason: Result was null at point of processing.");

            var playerStream = context.Player?.GetStream() ?? throw new InvalidOperationException("Could not send execution message. Reason: Player reference was null at point of processing.");

            if (string.IsNullOrEmpty(context.ExecutionMessage))
                return;

            await _messageService.SendMessageAsync(playerStream, context.ExecutionMessage, cancellationToken);

            if (string.IsNullOrEmpty(result.AdditionalMessage))
                return;

            await _messageService.SendMessageAsync(playerStream, context.ExecutionMessage, cancellationToken);

            await next();
        }
    }
}