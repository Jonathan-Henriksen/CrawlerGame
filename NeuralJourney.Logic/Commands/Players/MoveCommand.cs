using NeuralJourney.Library.Attributes;
using NeuralJourney.Library.Enums;
using NeuralJourney.Library.Enums.Commands;
using NeuralJourney.Library.Exceptions.Commands;
using NeuralJourney.Library.Exceptions.PlayerActions;
using NeuralJourney.Library.Models.CommandInfo;
using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Commands.Players.Base;
using NeuralJourney.Logic.Options;

namespace NeuralJourney.Logic.Commands.Players
{
    [PlayerCommand(PlayerCommandEnum.Move)]
    internal class MoveCommand : PlayerCommand
    {
        private readonly Player Player;

        private readonly DirectionEnum Direction;

        private readonly int WorldHeight;
        private readonly int WorldWidth;

        internal MoveCommand(PlayerCommandInfo commandInfo, GameOptions gameOptions) : base(commandInfo)
        {
            Player = commandInfo.Player;

            WorldHeight = gameOptions.WorldHeight;
            WorldWidth = gameOptions.WorldWidth;

            if (commandInfo.Params is null || commandInfo.Params.Length < 1)
                throw new MissingParameterException(nameof(Direction));

            if (!Enum.TryParse(commandInfo.Params[0], out DirectionEnum direction))
                throw new InvalidParameterException($"{commandInfo.CommandEnum}", nameof(Direction), commandInfo.Params[0], "North/South/East/West");

            Direction = direction;
        }

        protected override void Execute()
        {
            switch (Direction)
            {
                case DirectionEnum.North:
                    Player.Location.Y = Move(Player.Location.Y, WorldHeight, 1);
                    break;

                case DirectionEnum.South:
                    Player.Location.Y = Move(Player.Location.Y, WorldHeight, -1);
                    break;

                case DirectionEnum.East:
                    Player.Location.X = Move(Player.Location.X, WorldWidth, 1);
                    break;

                case DirectionEnum.West:
                    Player.Location.X = Move(Player.Location.X, WorldWidth, -1);
                    break;
            }
        }

        private int Move(int coordinate, int boundary, int increment)
        {
            if ((increment > 0 && coordinate < boundary - 1) || (increment < 0 && coordinate > 0))
                return coordinate + increment;
            else
                throw new MapLimitReachedException(Player, Direction);
        }
    }
}