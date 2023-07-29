using CrawlerGame.Library.Enums;
using CrawlerGame.Library.Models.Player;
using CrawlerGame.Logic.Commands.PlayerCommands;
using CrawlerGame.Logic.Commands.Interfaces;
using CrawlerGame.Logic.Factories.Interfaces;

namespace CrawlerGame.Logic.Factories
{
    public class CommandFactory : ICommandFactory
    {
        public ICommand GetPlayerCommand(Player player, CommandEnum? command)
        {
            return command switch
            {
                CommandEnum.MoveNorth => new MovePlayerCommand(player, Direction.North),
                CommandEnum.MoveSouth => new MovePlayerCommand(player, Direction.South),
                CommandEnum.MoveEast => new MovePlayerCommand(player, Direction.East),
                CommandEnum.MoveWest => new MovePlayerCommand(player, Direction.North),
                _ => new UnknownCommand()
            };
        }
    }
}