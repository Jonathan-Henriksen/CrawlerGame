using NeuralJourney.Library.Enums;
using NeuralJourney.Library.Models.CommandInfo.Base;

namespace NeuralJourney.Library.Models.CommandInfo
{
    public class AdminCommandInfo : CommandInfoBase<AdminCommandEnum>
    {
        public AdminCommandInfo(AdminCommandEnum command, string[]? @params, string successMessage) : base(command, @params, successMessage)
        {

        }
    }
}