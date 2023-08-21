using NeuralJourney.Core.Exceptions;
using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Interfaces.Services;
using NeuralJourney.Core.Models.LogProperties;

namespace NeuralJourney.Core.Commands.Players.Middleware
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
            if (context.Result is null || string.IsNullOrEmpty(context.ExecutionMessage))
                throw new CommandExecutionException("No execution message generated");

            var client = context.Player?.Client;

            if (client is null)
                return;

            await _messageService.SendMessageAsync(client, context.ExecutionMessage, cancellationToken);

            if (context.Result.HasValue && !string.IsNullOrEmpty(context.Result.Value.AdditionalMessage))
                await _messageService.SendMessageAsync(client, context.Result.Value.AdditionalMessage, cancellationToken);

            await next();
        }
    }
}