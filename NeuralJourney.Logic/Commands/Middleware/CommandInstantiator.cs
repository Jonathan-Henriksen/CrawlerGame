using NeuralJourney.Library.Enums.Commands;
using NeuralJourney.Library.Exceptions.Commands;
using NeuralJourney.Library.Models.Commands;

namespace NeuralJourney.Logic.Commands.Middleware
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