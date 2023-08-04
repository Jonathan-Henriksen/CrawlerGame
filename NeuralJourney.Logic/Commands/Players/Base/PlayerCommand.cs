using NeuralJourney.Library.Extensions;
using NeuralJourney.Library.Models.CommandInfo;
using System.Net.Sockets;

namespace NeuralJourney.Logic.Commands.Players.Base
{
    public abstract class PlayerCommand : CommandBase
    {
        protected readonly NetworkStream? ResponseStream;

        protected PlayerCommand(PlayerCommandInfo commandInfo) : base(commandInfo.Params, commandInfo.SuccessMessage, commandInfo.FailureMessage)
        {
            ResponseStream = commandInfo.Player?.GetStream();
        }

        protected override async Task SendResponseAsync(string responseMessage)
        {
            await ResponseStream.SendMessageAsync(responseMessage);
        }
    }
}