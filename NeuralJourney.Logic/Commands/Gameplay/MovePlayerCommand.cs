using NeuralJourney.Library.Enums;
using NeuralJourney.Library.Models.ChatGPT;
using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Commands.Base;

namespace NeuralJourney.Logic.Commands.Gameplay
{
    internal class MovePlayerCommand : Command
    {
        private readonly Player? _player;
        private readonly Direction _direction;
        private readonly int _worldHeight;
        private readonly int _worldWidth;

        public MovePlayerCommand(CommandInfo commandInfo, Direction direction, int worldHeight, int worldWidth) : base(commandInfo)
        {
            _player = commandInfo?.Player;
            _direction = direction;
            _worldHeight = worldHeight;
            _worldWidth = worldWidth;
        }

        protected override bool Execute()
        {
            if (_player?.Location is null)
                return false;

            switch (_direction)
            {
                case Direction.North:
                    if (_player.Location.Y < _worldHeight - 1)
                        _player.Location.Y++;
                    break;

                case Direction.South:
                    if (_player.Location.Y > 0)
                        _player.Location.Y--;
                    break;

                case Direction.East:
                    if (_player.Location.X < _worldWidth - 1)
                        _player.Location.X++;
                    break;

                case Direction.West:
                    if (_player.Location.X > 0)
                        _player.Location.X--;
                    break;

                default:
                    return false;
            }

            return true;
        }
    }
}