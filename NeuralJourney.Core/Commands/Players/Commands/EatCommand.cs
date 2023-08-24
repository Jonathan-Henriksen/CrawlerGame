using NeuralJourney.Core.Enums.Commands;
using NeuralJourney.Core.Models.Commands;
using NeuralJourney.Core.Models.LogProperties;
using NeuralJourney.Core.Models.Options;

namespace NeuralJourney.Core.Commands.Players.Commands
{
    [Command(CommandTypeEnum.Player, CommandIdentifierEnum.Eat)]
    public class EatCommand : CommandBase
    {
        private readonly string? _foodItem;

        public EatCommand(CommandContext context, GameOptions gameOptions) : base(context, gameOptions)
        {
            _foodItem = context.Params.FirstOrDefault();
        }
        public override Task<CommandResult> ExecuteAsync()
        {
            if (_foodItem is null)
                return Task.FromResult(new CommandResult(false, Context.ExecutionMessage));

            return Task.FromResult(new CommandResult(true, $"No action is implemented for eating {_foodItem}"));
        }
    }
}