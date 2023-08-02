using NeuralJourney.Library.Models.ChatGPT;
using NeuralJourney.Logic.Commands.Base;

namespace NeuralJourney.Logic.Commands.System
{
    internal class UnknownCommand : Command
    {
        private readonly bool _isAdmin;

        public UnknownCommand(CommandInfo commandInfo, bool isAdmin = false) : base(commandInfo)
        {
            _isAdmin = isAdmin;
        }

        protected override bool Execute()
        {
            if (_isAdmin)
                Console.WriteLine(SuccessMessage);

            return true;
        }
    }
}