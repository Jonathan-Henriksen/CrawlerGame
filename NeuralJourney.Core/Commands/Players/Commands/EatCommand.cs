using NeuralJourney.Core.Enums.Commands;
using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Models.Commands;
using NeuralJourney.Core.Models.LogProperties;
using NeuralJourney.Core.Models.Options;

namespace NeuralJourney.Core.Commands.Players.Commands
{
    [Command(CommandTypeEnum.Player, CommandIdentifierEnum.Eat)]
    public class EatCommand : ICommand
    {
        private readonly CommandContext _context;

        public EatCommand(CommandContext context, GameOptions gameOptions)
        {
            _context = context;
        }
        public Task<CommandResult> ExecuteAsync()
        {
            return Task.FromResult<CommandResult>(new CommandResult("Not implemented yet"));
        }
    }
}