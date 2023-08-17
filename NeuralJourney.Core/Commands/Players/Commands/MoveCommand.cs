using NeuralJourney.Core.Enums.Commands;
using NeuralJourney.Core.Enums.Parameters;
using NeuralJourney.Core.Exceptions;
using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Models.Commands;
using NeuralJourney.Core.Models.Options;

namespace NeuralJourney.Core.Commands.Players.Commands
{
    [Command(CommandTypeEnum.Player, CommandIdentifierEnum.Move)]
    internal class MoveCommand : ICommand
    {
        private readonly CommandContext _context;

        private readonly DirectionEnum _direction;

        private readonly int _worldHeight;
        private readonly int WorldWidth;

        internal MoveCommand(CommandContext context, GameOptions gameOptions)
        {
            if (!context.Params.Any())
                throw new CommandExecutionException("Missing required parameter 'Direction'", "Could not determine which direction to move");

            if (!Enum.TryParse(context.Params[0], out DirectionEnum direction))
                throw new CommandExecutionException("Could not parse 'Direction' parameter to DirectionEnum", "Could not determine which direction to move");

            _context = context;

            _direction = direction;

            _worldHeight = gameOptions.WorldHeight;
            WorldWidth = gameOptions.WorldWidth;
        }

        public Task<CommandResult> ExecuteAsync()
        {
            if (_context.Player is null)
                throw new CommandExecutionException("The player was null", "Something went wrong. Please try again");

            return Task.Run(() =>
            {
                var playerLocation = _context.Player.Location;

                switch (_direction)
                {
                    case DirectionEnum.North:
                        playerLocation.Y = Move(playerLocation.Y, _worldHeight, 1);
                        break;

                    case DirectionEnum.South:
                        playerLocation.Y = Move(playerLocation.Y, _worldHeight, -1);
                        break;

                    case DirectionEnum.East:
                        playerLocation.X = Move(playerLocation.X, WorldWidth, 1);
                        break;

                    case DirectionEnum.West:
                        playerLocation.X = Move(playerLocation.X, WorldWidth, -1);
                        break;
                }

                return new CommandResult();
            });
        }

        private int Move(int coordinate, int boundary, int increment)
        {
            if (increment > 0 && coordinate < boundary - 1 || increment < 0 && coordinate > 0)
                return coordinate + increment;
            else
                throw new CommandExecutionException("Player tried to move beyond map boundriesl", $"You cannot go any further {_direction}");
        }
    }
}