using CrawlerGame.Library.Enums;
using CrawlerGame.Library.Models.ChatGPT;
using CrawlerGame.Library.Models.Player;
using CrawlerGame.Logic.Commands.Base;

namespace CrawlerGame.Logic.Commands.Gameplay
{
    internal class MovePlayerCommand : Command
    {
        private readonly Player _player;
        private readonly Direction _direction;

        public MovePlayerCommand(Player player, Direction direction, CommandInfo commandInfo) : base(commandInfo, player.GetStream())
        {
            _player = player;
            _direction = direction;
        }

        protected override bool ExecuteSpecific()
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