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

        protected Command(CommandInfo commandInfo, NetworkStream? responseStream)
        {
            SuccessMessage = commandInfo.SuccessMessage;
            FailureMessage = commandInfo.FailureMessage;
            ResponseStream = responseStream;
        }

        internal async Task ExecuteAsync()
        {
            string responseMessage;
            if (ExecuteSpecific())
            {
                responseMessage = SuccessMessage;
            }
            else
            {
                responseMessage = FailureMessage;
            }

            await ResponseStream.SendMessageAsync(responseMessage);
        }

        protected abstract bool ExecuteSpecific();
    }
}
