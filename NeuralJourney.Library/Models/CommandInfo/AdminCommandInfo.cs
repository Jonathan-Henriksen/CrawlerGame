using NeuralJourney.Library.Enums;
using NeuralJourney.Library.Models.CommandInfo.Base;

namespace NeuralJourney.Library.Models.CommandInfo
{
    public class AdminCommandInfo : CommandInfoBase
    {
        public AdminCommandInfo(AdminCommandEnum command, string[]? @params, string successMessage, string failureMessage) : base(command, @params, successMessage, failureMessage)
        {

        }
    }
}