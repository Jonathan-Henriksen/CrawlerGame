using NeuralJourney.Core.Attributes;
using NeuralJourney.Core.Enums.Commands;
using NeuralJourney.Core.Exceptions;
using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Models.Commands;
using NeuralJourney.Core.Options;

namespace NeuralJourney.Core.Commands.Players.Commands
{
    [Command(CommandTypeEnum.Player, CommandIdentifierEnum.CheckMap)]
    internal class CheckMapCommand : ICommand
    {
        private readonly CommandContext _context;

        private readonly int _worldWidth;
        private readonly int _worldHeight;

        internal CheckMapCommand(CommandContext context, GameOptions gameOptions)
        {
            _context = context;

            _worldWidth = gameOptions.WorldWidth;
            _worldHeight = gameOptions.WorldHeight;
        }

        public Task<CommandResult> ExecuteAsync()
        {
            return Task.Run(() =>
            {
                if (_context.Player is null)
                    throw new CommandExecutionException("The player was null");

                var map = new string('#', _worldWidth + 2) + "\n";
                for (var y = 0; y < _worldHeight; y++)
                {
                    map += "#";
                    for (var x = 0; x < _worldWidth; x++)
                    {
                        if (x == _context.Player.Location.X && y == _context.Player.Location.Y)
                            map += "P"; // 'P' for player
                        else
                        {
                            map += "."; // '.' for an empty room
                        }
                    }
                    map += "#\n";
                }
                map += new string('#', _worldWidth + 2) + "\n";

                return new CommandResult(map);
            });
        }
    }
}