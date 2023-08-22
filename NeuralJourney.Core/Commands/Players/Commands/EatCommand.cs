using NeuralJourney.Core.Enums.Commands;
using NeuralJourney.Core.Models.Commands;
using NeuralJourney.Core.Models.LogProperties;
using NeuralJourney.Core.Models.Options;

namespace NeuralJourney.Core.Commands.Players.Commands
{
    [Command(CommandTypeEnum.Player, CommandIdentifierEnum.Eat)]
    public class EatCommand : CommandBase
    {
        private readonly string _foodItem;
        public EatCommand(CommandContext context, GameOptions gameOptions, string foodItem) : base(context, gameOptions)
        {
            _foodItem = foodItem;
        }
        public override Task<CommandResult> ExecuteAsync()
        {
            return Task.FromResult(new CommandResult($"No action is implemented for eating {_foodItem}"));
        }
    }
}