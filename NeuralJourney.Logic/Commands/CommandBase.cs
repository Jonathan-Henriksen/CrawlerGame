using NeuralJourney.Library.Models.Commands;

namespace NeuralJourney.Logic.Commands
{
    public abstract class CommandBase
    {
        protected readonly CommandContext Context;

        protected CommandBase(CommandContext context)
        {
            Context = context;
        }

        internal abstract Task<CommandResult> ExecuteAsync();
    }
}