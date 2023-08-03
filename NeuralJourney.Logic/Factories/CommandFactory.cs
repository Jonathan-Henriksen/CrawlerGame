using NeuralJourney.Library.Constants;
using NeuralJourney.Library.Enums;
using NeuralJourney.Library.Models.OpenAI;
using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Commands.Base;
using NeuralJourney.Logic.Commands.Gameplay;
using NeuralJourney.Logic.Commands.System;
using NeuralJourney.Logic.Engines.Interfaces;
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
                CommandEnum.CheckMap => new CheckMapCommand(commandInfo, _gameOptions),
                CommandEnum.Move => GetMovePlayerCommand(commandInfo),
                _ => new UnknownCommand(commandInfo)
            };
        }

        public Command GetAdminCommand(IGameEngine game, string adminInput, params string[] parameters)
        {
            if (!adminInput.StartsWith('/') || !Enum.TryParse(adminInput[1..], true, out AdminCommandEnum command))
                return GetUnknownAdminCommand();

            return command switch
            {
                _ => GetUnknownAdminCommand()
            };
        }

        private static UnknownCommand GetUnknownAdminCommand()
        {
            var commandInfo = new CommandInfo(CommandEnum.Unknown, Phrases.Failure.UnknownCommand);
            return new UnknownCommand(commandInfo, true);
        }

        private MovePlayerCommand GetMovePlayerCommand(CommandInfo commandInfo)
        {
            DirectionEnum? direction = Enum.TryParse(commandInfo.Params.FirstOrDefault(), out DirectionEnum directionParam) ? directionParam : (DirectionEnum?) null;

            return new MovePlayerCommand(commandInfo, direction, _gameOptions.WorldHeight, _gameOptions.WorldWidth);
        }
    }
}