using NeuralJourney.Library.Enums;

namespace NeuralJourney.Library.Models.CommandInfo.Base
{
    public class CommandInfoBase
    {
        public CommandInfoBase(AdminCommandEnum command, string[]? @params, string successMessage, string failureMessage)
        {
            AdminCommand = command;
            Params = @params;
            SuccessMessage = successMessage;
            FailureMessage = failureMessage;
        }

        public CommandInfoBase(PlayerCommandEnum command, string[]? @params, string successMessage, string failureMessage)
        {
            PlayerCommand = command;
            Params = @params;
            SuccessMessage = successMessage;
            FailureMessage = failureMessage;
        }

        public AdminCommandEnum AdminCommand { get; set; }

        public PlayerCommandEnum PlayerCommand { get; set; }

        public string[]? Params { get; set; }

        public string SuccessMessage { get; set; }

        public string FailureMessage { get; set; }
    }
}