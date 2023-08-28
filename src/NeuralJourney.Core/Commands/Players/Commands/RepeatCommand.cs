using NeuralJourney.Core.Enums.Commands;
using NeuralJourney.Core.Models.Commands;
using NeuralJourney.Core.Models.LogProperties;
using NeuralJourney.Core.Models.Options;

namespace NeuralJourney.Core.Commands.Players.Commands
{
    [Command(CommandTypeEnum.Player, CommandIdentifierEnum.Repeat)]
    public class RepeatCommand : CommandBase
    {
        private readonly CommandContext? _previousCommand;

        public RepeatCommand(CommandContext context, GameOptions gameOptions) : base(context, gameOptions)
        {
            _previousCommand = context.Player?.PreviousCommands.FirstOrDefault();
        }
        public override Task<CommandResult> ExecuteAsync()
        {
            if (_previousCommand is null)
                return Task.FromResult(new CommandResult(false, Context.ExecutionMessage));

            return Task.FromResult(new CommandResult(true, $"Executing the command {_previousCommand.CommandKey.Identifier} again"));
        }
    }
}