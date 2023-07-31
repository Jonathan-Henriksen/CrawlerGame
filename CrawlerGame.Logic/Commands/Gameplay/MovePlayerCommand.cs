using CrawlerGame.Library.Enums;
using CrawlerGame.Library.Models.Player;
using CrawlerGame.Logic.Commands.Interfaces;

namespace CrawlerGame.Logic.Commands.Gameplay
{
    internal class MovePlayerCommand : ICommand
    {
        private readonly Player _player;
        private readonly Direction _direction;

        public MovePlayerCommand(Player player, Direction direction)
        {
            _player = player;
            _direction = direction;
        }

        public bool Execute()
        {
            if (_player.Location is null)
                return false;

            switch (_direction)
            {
                case Direction.North:
                    _player.Location.Y++;
                    break;
                case Direction.South:
                    _player.Location.Y--;
                    break;
                case Direction.East:
                    _player.Location.X++;
                    break;
                case Direction.West:
                    _player.Location.X--;
                    break;
                default:
                    return false;
            }

            return true;
        }
    }
}