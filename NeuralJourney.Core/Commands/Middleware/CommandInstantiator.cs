using NeuralJourney.Core.Enums.Commands;
using NeuralJourney.Core.Exceptions.Commands;
using NeuralJourney.Core.Interfaces.Commands;

namespace NeuralJourney.Core.Commands.Middleware
{
    public class CommandInstantiator : ICommandMiddleware
    {
        private readonly ICommandFactory _commandFactory;

        public CommandInstantiator(ICommandFactory commandFactory)
        {
            _commandFactory = commandFactory;
        }

        public async Task InvokeAsync(CommandContext context, Func<Task> next, CancellationToken cancellationToken = default)
        {
            var commandKey = context.CommandKey ?? throw new MissingParameterException(CommandIdentifierEnum.Unknown, nameof(context.CommandKey));

            context.Command = _commandFactory.CreateCommand(commandKey, context.Params);

            await next();
        }
    }
}