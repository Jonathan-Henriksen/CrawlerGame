using NeuralJourney.Library.Extensions;
using NeuralJourney.Library.Models.CommandInfo;
using System.Net.Sockets;

namespace NeuralJourney.Logic.Commands.Players.Base
{
    public abstract class PlayerCommand
    {
        protected readonly string SuccessMessage;
        protected readonly string FailureMessage;
        protected readonly NetworkStream? ResponseStream;

        protected PlayerCommand(PlayerCommandInfo commandInfo)
        {
            SuccessMessage = commandInfo.SuccessMessage;
            FailureMessage = commandInfo.FailureMessage ?? string.Empty;
            ResponseStream = commandInfo.Player?.GetStream();
        }

        internal async Task ExecuteAsync()
        {
            var (success, callback) = Execute();

            var responseMessage = success ? SuccessMessage : FailureMessage;

            await ResponseStream.SendMessageAsync(responseMessage);

            callback?.Invoke();
        }

        protected abstract (bool Success, Action? Callback) Execute();
    }
}