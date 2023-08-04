using NeuralJourney.Library.Models.CommandInfo;

namespace NeuralJourney.Logic.Commands.Admin.Base
{
    public abstract class AdminCommand : CommandBase
    {
        protected AdminCommand(AdminCommandInfo commandInfo) : base(commandInfo.Params, commandInfo.SuccessMessage, commandInfo.FailureMessage)
        {
        }

        protected override Task SendResponseAsync(string responseMessage)
        {
            Console.WriteLine(responseMessage);
            return Task.CompletedTask;
        }
    }
}