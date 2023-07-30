using CrawlerGame.Library.Enums;
using CrawlerGame.Library.Models.Player;
using CrawlerGame.Logic.Commands.Base;

namespace CrawlerGame.Logic.Commands.PlayerCommands
{
    internal class MovePlayerCommand : Command
    {
        private readonly Player _player;
        private readonly Direction _direction;

        public MovePlayerCommand(Player player, Direction direction, string successMessage = "", string failureMessage = "") : base(successMessage, failureMessage)
        {
            _player = player;
            _direction = direction;
        }

        public override void Execute()
        {
            _player.Move(_direction);
        }
    }
}