using NeuralJourney.Library.Constants;
using NeuralJourney.Library.Enums;
using NeuralJourney.Library.Models.ChatGPT;
using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Commands.Base;
using NeuralJourney.Logic.Commands.Gameplay;
using NeuralJourney.Logic.Commands.System;
using NeuralJourney.Logic.Factories.Interfaces;
using NeuralJourney.Logic.Options;

namespace NeuralJourney.Logic.Factories
{
    public class CommandFactory : ICommandFactory
    {
        private readonly GameOptions _gameOptions;

        public CommandFactory(GameOptions gameOptions)
        {
            _gameOptions = gameOptions;
        }

        public Command GetPlayerCommand(Player player, CommandInfo commandInfo)
        {
            commandInfo.Player = player;

            return commandInfo.Command switch
            {
                CommandEnum.MoveNorth => GetMovePlayerCommand(commandInfo, Direction.North),
                CommandEnum.MoveSouth => GetMovePlayerCommand(commandInfo, Direction.South),
                CommandEnum.MoveEast => GetMovePlayerCommand(commandInfo, Direction.East),
                CommandEnum.MoveWest => GetMovePlayerCommand(commandInfo, Direction.North),
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
            var commandInfo = new CommandInfo(CommandEnum.Unknown, Phrases.UnknownCommand);
            return new UnknownCommand(commandInfo, true);
        }

        private MovePlayerCommand GetMovePlayerCommand(CommandInfo commandInfo, Direction direction)
        {
            return new MovePlayerCommand(commandInfo, direction, _gameOptions.WorldHeight, _gameOptions.WorldWidth);
        }
    }
}