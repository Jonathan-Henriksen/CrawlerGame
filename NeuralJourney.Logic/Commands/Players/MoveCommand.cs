using NeuralJourney.Library.Attributes;
using NeuralJourney.Library.Constants;
using NeuralJourney.Library.Enums;
using NeuralJourney.Library.Models.CommandInfo;
using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Commands.Players.Base;
using NeuralJourney.Logic.Options;

namespace NeuralJourney.Logic.Commands.Players
{
    [PlayerCommand(PlayerCommandEnum.CheckMap)]
    internal class MoveCommand : PlayerCommand
    {
        private readonly Player _player;
        private readonly DirectionEnum? _direction;
        private readonly int _worldHeight;
        private readonly int _worldWidth;

        public MoveCommand(PlayerCommandInfo commandInfo, GameOptions gameOptions) : base(commandInfo)
        {
            _player = commandInfo.Player;
            _worldHeight = gameOptions.WorldHeight;
            _worldWidth = gameOptions.WorldWidth;

            commandInfo.FailureMessage = string.Format(Phrases.Failure.MovePlayer, _direction);

            if (commandInfo.Params is not null && Enum.TryParse(commandInfo.Params[0], out DirectionEnum direction))
                _direction = direction;
        }

        protected override (bool Success, Action? Callback) Execute()
        {
            if (_player.Location is null)
                return (false, null);

            switch (_direction)
            {
                case DirectionEnum.North:
                    if (_player.Location.Y < _worldHeight - 1)
                        _player.Location.Y++;
                    break;

                case DirectionEnum.South:
                    if (_player.Location.Y > 0)
                        _player.Location.Y--;
                    break;

                case DirectionEnum.East:
                    if (_player.Location.X < _worldWidth - 1)
                        _player.Location.X++;
                    break;

                case DirectionEnum.West:
                    if (_player.Location.X > 0)
                        _player.Location.X--;
                    break;

                default:
                    return (false, null);
            }

            return (true, null);
        }
    }
}