using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Models.LogProperties;
using NeuralJourney.Core.Models.Options;

namespace NeuralJourney.Core.Models.Commands
{
    public abstract class CommandBase : ICommand
    {
        protected CommandContext Context;

        protected GameOptions Options;

        protected CommandBase(CommandContext context, GameOptions options)
        {
            Context = context;
            Options = options;
        }

        public abstract Task<CommandResult> ExecuteAsync();
    }
}