using NeuralJourney.Core.Exceptions;
using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Models.Commands;

namespace NeuralJourney.Core.Commands.Players.Middleware
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
            var commandKey = context.CommandKey ?? throw new CommandMappingException("CommandKey was null");

            context.Command = _commandFactory.CreateCommand(commandKey, context.Params);

            await next();
        }
    }
}