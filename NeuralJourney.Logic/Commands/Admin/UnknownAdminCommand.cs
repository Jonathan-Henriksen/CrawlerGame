using NeuralJourney.Library.Attributes;
using NeuralJourney.Library.Constants;
using NeuralJourney.Library.Enums;
using NeuralJourney.Library.Models.CommandInfo;
using NeuralJourney.Logic.Commands.Admin.Base;

namespace NeuralJourney.Logic.Commands.Admin
{
    [PlayerCommand(PlayerCommandEnum.Unknown)]
    internal class UnknownAdminCommand : AdminCommand
    {
        public UnknownAdminCommand(AdminCommandInfo commandInfo) : base(commandInfo)
        {
            commandInfo.FailureMessage = Phrases.Failure.UnknownCommand;
        }

        protected override bool Execute()
        {
            return true;
        }
    }
}