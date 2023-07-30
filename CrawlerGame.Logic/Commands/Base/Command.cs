namespace CrawlerGame.Logic.Commands.Base
{
    internal abstract class Command
    {
        internal Command(string successMessage, string failureMesasge)
        {
            SuccessMessage = successMessage;
            FailureMessage = failureMesasge;
        }

        internal string SuccessMessage { get; init; }

        internal string FailureMessage { get; init; }

        public abstract void Execute();
    }
}