using NeuralJourney.Core.Attributes;
using NeuralJourney.Core.Enums.Commands;
using NeuralJourney.Core.Enums.Parameters;
using NeuralJourney.Core.Exceptions.Commands;
using NeuralJourney.Core.Exceptions.PlayerActions;
using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Options;

namespace NeuralJourney.Core.Commands.Players
{
    [Command(CommandTypeEnum.Player, CommandIdentifierEnum.Move)]
    internal class MoveCommand : ICommand
    {
        private readonly CommandContext _context;

        private readonly DirectionEnum Direction;

        private readonly int WorldHeight;
        private readonly int WorldWidth;

        internal MoveCommand(CommandContext context, GameOptions gameOptions)
        {
            if (context.Params is null || context.Params.Length < 1)
                throw new MissingParameterException(context.CommandKey?.Identifier ?? default, nameof(Direction));

            if (!Enum.TryParse(context.Params[0], out DirectionEnum direction))
                throw new InvalidParameterException(context.CommandKey?.Identifier ?? default, nameof(Direction), context.Params[0],
                    string.Format("{0}, {1}, {2}, {3}", DirectionEnum.North, DirectionEnum.South, DirectionEnum.East, DirectionEnum.West));

            _context = context;

            Direction = direction;

            WorldHeight = gameOptions.WorldHeight;
            WorldWidth = gameOptions.WorldWidth;
        }

        public Task<CommandResult> ExecuteAsync()
        {
            if (_context.Player is null)
                throw new MissingParameterException(_context.CommandKey?.Identifier ?? default, nameof(_context.Player));

            return Task.Run(() =>
            {
                var playerLocation = _context.Player.Location;

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
            if (increment > 0 && coordinate < boundary - 1 || increment < 0 && coordinate > 0)
                return coordinate + increment;
            else
                throw new MapLimitReachedException(_context.Player!, Direction); // Player can't be null due to checks in the calling function.
        }
    }
}