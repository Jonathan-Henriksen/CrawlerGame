using CrawlerGame.Library.Extensions;
using CrawlerGame.Library.Models.ChatGPT;
using System.Net.Sockets;

namespace CrawlerGame.Logic.Commands.Base
{
    public abstract class Command
    {
        protected readonly string SuccessMessage;
        protected readonly string FailureMessage;
        protected readonly TcpClient? ResponseClient;

        protected Command(CommandInfo commandInfo)
        {
            SuccessMessage = commandInfo.SuccessMessage;
            FailureMessage = commandInfo.FailureMessage;
            ResponseClient = commandInfo.Player?.GetClient();
        }

        internal async Task ExecuteAsync()
        {
            string responseMessage;

            if (Execute())
                responseMessage = SuccessMessage;
            else
                responseMessage = FailureMessage;

            using var responseStream = ResponseClient?.GetStream();

            await responseStream.SendMessageAsync(responseMessage);
        }

        protected abstract bool Execute();
    }
}