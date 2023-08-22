using NeuralJourney.Core.Enums.Commands;
using NeuralJourney.Core.Enums.Parameters;
using NeuralJourney.Core.Exceptions;
using NeuralJourney.Core.Models.Commands;
using NeuralJourney.Core.Models.LogProperties;
using NeuralJourney.Core.Models.Options;

namespace NeuralJourney.Core.Commands.Players.Commands
{
    [Command(CommandTypeEnum.Player, CommandIdentifierEnum.Move)]
    public class MoveCommand : CommandBase
    {
        private readonly DirectionEnum _direction;

        private readonly int _worldHeight;
        private readonly int WorldWidth;

        public MoveCommand(CommandContext context, GameOptions gameOptions, string direction) : base(context, gameOptions)
        {
            if (!context.Params.Any())
                throw new CommandExecutionException("Missing required parameter 'Direction'", "Could not determine which direction to move");

            if (!Enum.TryParse(direction, true, out DirectionEnum directionEnum))
                throw new CommandExecutionException("Could not parse 'Direction' parameter to DirectionEnum", "Could not determine which direction to move");

            _direction = directionEnum;

            _worldHeight = gameOptions.WorldHeight;
            WorldWidth = gameOptions.WorldWidth;
        }

        public override Task<CommandResult> ExecuteAsync()
        {
            if (Context.Player is null)
                throw new CommandExecutionException("The player was null", "Something went wrong. Please try again");

            return Task.Run(() =>
            {
                var playerLocation = Context.Player.Location;

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