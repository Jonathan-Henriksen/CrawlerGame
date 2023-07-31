using CrawlerGame.Library.Enums;
using CrawlerGame.Library.Models.Player;
using CrawlerGame.Logic.Commands.Gameplay;
using CrawlerGame.Logic.Commands.Interfaces;
using CrawlerGame.Logic.Commands.System;
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

        public ICommand GetAdminCommand(IGameEngine game, string adminInput, params string[] parameters)
        {
            if (!adminInput.StartsWith('/') || !Enum.TryParse(adminInput[1..], true, out AdminCommandEnum command))
                return new UnknownCommand();

            return command switch
            {
                _ => new UnknownCommand()
            };
        }
    }
}