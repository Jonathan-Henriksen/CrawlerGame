using CrawlerGame.Library.Extensions;
using CrawlerGame.Library.Models.ChatGPT;
using System.Net.Sockets;

namespace CrawlerGame.Logic.Commands.Base
{
    public abstract class Command
    {
        protected readonly string SuccessMessage;
        protected readonly string FailureMessage;
        protected readonly NetworkStream? ResponseStream;

        protected Command(CommandInfo commandInfo)
        {
            SuccessMessage = commandInfo.SuccessMessage;
            FailureMessage = commandInfo.FailureMessage;
            ResponseStream = commandInfo.Player?.GetStream();
        }

        internal async Task ExecuteAsync()
        {
            string responseMessage;

            if (Execute())
                responseMessage = SuccessMessage;
            else
                responseMessage = FailureMessage;

            await ResponseStream.SendMessageAsync(responseMessage);
        }

        protected abstract bool Execute();
    }
}