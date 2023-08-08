using NeuralJourney.Library.Attributes;
using NeuralJourney.Library.Enums.Commands;
using NeuralJourney.Library.Enums.Parameters;
using NeuralJourney.Library.Exceptions.Commands;
using NeuralJourney.Library.Exceptions.PlayerActions;
using NeuralJourney.Library.Models.Commands;
using NeuralJourney.Logic.Options;

namespace NeuralJourney.Logic.Commands.Players
{
    [Command(CommandTypeEnum.Player, CommandIdentifierEnum.Move)]
    internal class MoveCommand : CommandBase
    {
        private readonly DirectionEnum Direction;

        private readonly int WorldHeight;
        private readonly int WorldWidth;

        internal MoveCommand(CommandContext commandContext, GameOptions gameOptions) : base(commandContext)
        {
            if (commandContext.Params is null || commandContext.Params.Length < 1)
                throw new MissingParameterException(commandContext.CommandIdentifier, nameof(Direction));

            if (!Enum.TryParse(commandContext.Params[0], out DirectionEnum direction))
                throw new InvalidParameterException(commandContext.CommandIdentifier, nameof(Direction), commandContext.Params[0],
                    string.Format("{0}, {1}, {2}, {3}", DirectionEnum.North, DirectionEnum.South, DirectionEnum.East, DirectionEnum.West));

            Direction = direction;

            WorldHeight = gameOptions.WorldHeight;
            WorldWidth = gameOptions.WorldWidth;
        }

        internal override Task<CommandResult> ExecuteAsync()
        {
            if (Context.Player is null)
                throw new MissingParameterException(Context.CommandIdentifier, nameof(Context.Player));

            return Task.Run(() =>
            {
                var playerLocation = Context.Player.Location;

                switch (Direction)
                {
                    case DirectionEnum.North:
                        playerLocation.Y = Move(playerLocation.Y, WorldHeight, 1);
                        break;

                    case DirectionEnum.South:
                        playerLocation.Y = Move(playerLocation.Y, WorldHeight, -1);
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
            if ((increment > 0 && coordinate < boundary - 1) || (increment < 0 && coordinate > 0))
                return coordinate + increment;
            else
                throw new MapLimitReachedException(Context.Player!, Direction); // Player can't be null due to checks in the calling function.
        }
    }
}