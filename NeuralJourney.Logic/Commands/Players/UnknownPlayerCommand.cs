using NeuralJourney.Library.Attributes;
using NeuralJourney.Library.Constants;
using NeuralJourney.Library.Enums;
using NeuralJourney.Library.Models.CommandInfo;
using NeuralJourney.Logic.Commands.Players.Base;

namespace NeuralJourney.Logic.Commands.Players
{
    [PlayerCommandMapping(PlayerCommandEnum.Unknown)]
    internal class UnknownPlayerCommand : PlayerCommand
    {
        private readonly bool _isAdmin;

        public UnknownPlayerCommand(PlayerCommandInfo commandInfo, bool isAdmin = false) : base(commandInfo)
        {
            _isAdmin = isAdmin;

            commandInfo.FailureMessage = Phrases.Failure.UnknownCommand;
        }

        protected override (bool Success, Action? Callback) Execute()
        {
            if (_isAdmin)
                Console.WriteLine(FailureMessage);

            return (true, null);
        }
    }
}