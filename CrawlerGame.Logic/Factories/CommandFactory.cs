using CrawlerGame.Library.Enums;
using CrawlerGame.Library.Models.ChatGPT;
using CrawlerGame.Library.Models.Player;
using CrawlerGame.Logic.Commands.Base;
using CrawlerGame.Logic.Commands.Gameplay;
using CrawlerGame.Logic.Commands.System;
using CrawlerGame.Logic.Factories.Interfaces;

namespace CrawlerGame.Logic.Factories
{
    public class CommandFactory : ICommandFactory
    {
        public Command GetPlayerCommand(Player player, CommandInfo commandInfo)
        {
            return commandInfo.Command switch
            {
                CommandEnum.MoveNorth => new MovePlayerCommand(player, Direction.North, commandInfo),
                CommandEnum.MoveSouth => new MovePlayerCommand(player, Direction.South, commandInfo),
                CommandEnum.MoveEast => new MovePlayerCommand(player, Direction.East, commandInfo),
                CommandEnum.MoveWest => new MovePlayerCommand(player, Direction.North, commandInfo),
                _ => new UnknownCommand(commandInfo, player.GetStream())
            };
        }

        public Command GetAdminCommand(IGameEngine game, string adminInput, params string[] parameters)
        {
            if (!adminInput.StartsWith('/') || !Enum.TryParse(adminInput[1..], true, out AdminCommandEnum command))
                return UnknownAdminCommand();

            return command switch
            {
                _ => UnknownAdminCommand()
            };
        }

        private static Command UnknownAdminCommand()
        {
            var commandInfo = new CommandInfo() { SuccessMessage = "Unknown command" };
            return new UnknownCommand(commandInfo, default, true);
        }
    }
}