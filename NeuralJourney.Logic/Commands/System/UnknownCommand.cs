using NeuralJourney.Library.Attributes;
using NeuralJourney.Library.Constants;
using NeuralJourney.Library.Enums;
using NeuralJourney.Library.Models.ChatGPT;
using NeuralJourney.Logic.Commands.Base;

namespace NeuralJourney.Logic.Commands.System
{
    [CommandMapping(CommandEnum.Unknown)]
    internal class UnknownCommand : Command
    {
        private readonly bool _isAdmin;

        public UnknownCommand(CommandInfo commandInfo, bool isAdmin = false) : base(commandInfo)
        {
            _isAdmin = isAdmin;

            commandInfo.FailureMessage = Phrases.Failure.UnknownCommand;
        }

        protected override (bool Success, Action? Callback) Execute()
        {
            if (_isAdmin)
                Console.WriteLine(FailureMessage);

            return (true, null);
        }
    }
}