using NeuralJourney.Library.Models.CommandInfo;

namespace NeuralJourney.Logic.Commands.Admin.Base
{
    public abstract class AdminCommand
    {
        public readonly string? SuccessMessage;
        public readonly string? FailureMessage;

        protected AdminCommand(AdminCommandInfo commandInfo)
        {

        }

        internal Task ExecuteAsync()
        {
            return Task.Run(() =>
            {
                if (Execute())
                    Console.WriteLine(SuccessMessage);
                else
                    Console.WriteLine(FailureMessage);
            });
        }

        protected abstract bool Execute();
    }
}