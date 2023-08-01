using CrawlerGame.Library.Enums;
using CrawlerGame.Library.Models.ChatGPT;
using CrawlerGame.Library.Models.World;
using CrawlerGame.Logic.Commands.Base;

namespace CrawlerGame.Logic.Commands.Gameplay
{
    internal class MovePlayerCommand : Command
    {
        private readonly Player? _player;
        private readonly Direction _direction;

        public MovePlayerCommand(CommandInfo commandInfo, Direction direction) : base(commandInfo)
        {
            _player = commandInfo?.Player;
            _direction = direction;
        }

        protected override bool Execute()
        {
            if (_player?.Location is null)
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