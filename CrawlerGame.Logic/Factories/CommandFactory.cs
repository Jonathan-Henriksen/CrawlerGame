using CrawlerGame.Library.Enums;
using CrawlerGame.Library.Models.ChatGPT;
using CrawlerGame.Logic.Commands.Base;
using CrawlerGame.Logic.Commands.Gameplay;
using CrawlerGame.Logic.Commands.System;
using CrawlerGame.Logic.Factories.Interfaces;

namespace CrawlerGame.Logic.Factories
{
    public class CommandFactory : ICommandFactory
    {
        public Command GetPlayerCommand(CommandInfo commandInfo)
        {
            return commandInfo.Command switch
            {
                CommandEnum.MoveNorth => new MovePlayerCommand(commandInfo, Direction.North),
                CommandEnum.MoveSouth => new MovePlayerCommand(commandInfo, Direction.South),
                CommandEnum.MoveEast => new MovePlayerCommand(commandInfo, Direction.East),
                CommandEnum.MoveWest => new MovePlayerCommand(commandInfo, Direction.North),
                _ => new UnknownCommand(commandInfo)
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
            return new UnknownCommand(commandInfo, true);
        }
    }
}