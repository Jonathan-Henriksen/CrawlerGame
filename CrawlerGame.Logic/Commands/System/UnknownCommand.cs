using CrawlerGame.Library.Models.ChatGPT;
using CrawlerGame.Logic.Commands.Base;
using System.Net.Sockets;

namespace CrawlerGame.Logic.Commands.System
{
    internal class UnknownCommand : Command
    {
        private readonly bool _isAdmin;

        public UnknownCommand(CommandInfo commandInfo, NetworkStream? responseStream, bool isAdmin = false) : base(commandInfo, responseStream)
        {
            _isAdmin = isAdmin;
        }

        protected override bool ExecuteSpecific()
        {
            if (_isAdmin)
                Console.WriteLine(SuccessMessage);

            return true;
        }
    }
}