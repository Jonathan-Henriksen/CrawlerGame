using NeuralJourney.Library.Attributes;
using NeuralJourney.Library.Constants;
using NeuralJourney.Library.Enums;
using NeuralJourney.Library.Models.CommandInfo;
using NeuralJourney.Logic.Commands.Players.Base;
using NeuralJourney.Logic.Options;

namespace NeuralJourney.Logic.Commands.Players
{
    [PlayerCommand(PlayerCommandEnum.Unknown)]
    internal class UnknownPlayerCommand : PlayerCommand
    {
        public UnknownPlayerCommand(PlayerCommandInfo commandInfo) : base(commandInfo)
        {
            commandInfo.FailureMessage = Phrases.Failure.UnknownCommand;
        }

        protected override (bool Success, Action? Callback) Execute()
        {
            return (true, null);
        }
    }
}