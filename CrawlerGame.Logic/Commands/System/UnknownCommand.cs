using CrawlerGame.Logic.Commands.Base;

namespace CrawlerGame.Logic.Commands.System
{
    internal class UnknownCommand : Command
    {
        public UnknownCommand(string successMessage = "", string failureMesasge = "") : base(successMessage, failureMesasge)
        {
        }

        public override void Execute()
        {
            Console.WriteLine("Unknown command");
        }
    }
}