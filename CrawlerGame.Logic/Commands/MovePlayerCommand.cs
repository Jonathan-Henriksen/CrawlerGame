using CrawlerGame.Library.Enums;
using CrawlerGame.Library.Models.Player;
using CrawlerGame.Logic.Commands.Interfaces;

namespace CrawlerGame.Logic.Commands
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

        public void Execute()
        {
            _player.Move(_direction);
        }
    }
}