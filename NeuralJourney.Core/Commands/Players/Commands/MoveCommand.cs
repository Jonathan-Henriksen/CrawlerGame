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
        private readonly DirectionEnum? _direction;

        private readonly int _worldHeight;
        private readonly int _worldWidth;

        public MoveCommand(CommandContext context, GameOptions gameOptions) : base(context, gameOptions)
        {
            if (context.Params.Any() && Enum.TryParse(context.Params.First(), true, out DirectionEnum directionEnum))
                _direction = directionEnum;

            _worldHeight = gameOptions.WorldHeight;
            _worldWidth = gameOptions.WorldWidth;
        }

        public override Task<CommandResult> ExecuteAsync()
        {
            if (Context.Player is null)
                throw new InvalidOperationException("Player was null");

            if (_direction is null)
            {
                Context.Player.HasIncompleteCommand = true;
                return Task.FromResult(new CommandResult(false, Context.ExecutionMessage));
            }

            return Task.Run(() =>
            {
                var playerLocation = Context.Player.Location;

                try
                {
                    switch (_direction)
                    {
                        case DirectionEnum.North:
                            playerLocation.Y = Move(playerLocation.Y, _worldHeight, 1);
                            break;

                        case DirectionEnum.South:
                            playerLocation.Y = Move(playerLocation.Y, _worldHeight, -1);
                            break;

                        case DirectionEnum.East:
                            playerLocation.X = Move(playerLocation.X, _worldWidth, 1);
                            break;

                        case DirectionEnum.West:
                            playerLocation.X = Move(playerLocation.X, _worldWidth, -1);
                            break;
                    }
                }
                catch (CommandExecutionException ex)
                {
                    return new CommandResult(true, ex.PlayerMessage);
                }

                return new CommandResult(true, Context.ExecutionMessage);
            });
        }

        private int Move(int coordinate, int boundary, int increment)
        {
            if (increment > 0 && coordinate < boundary - 1 || increment < 0 && coordinate > 0)
                return coordinate + increment;
            else
                throw new CommandExecutionException("Player tried to move beyond map boundries", $"You cannot go any further {_direction}");
        }
    }
}