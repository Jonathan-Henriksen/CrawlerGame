using NeuralJourney.Library.Attributes;
using NeuralJourney.Library.Enums.Commands;
using NeuralJourney.Library.Exceptions.Commands;
using NeuralJourney.Library.Extensions;
using NeuralJourney.Library.Models.CommandContext;
using NeuralJourney.Library.Models.World;

namespace NeuralJourney.Logic.Commands.Players
{
    [CommandType(CommandTypeEnum.Player)]
    public abstract class PlayerCommandBase : CommandBase
    {
        protected readonly Player Player;

        protected PlayerCommandBase(CommandContext commandContext) : base(commandContext.Params, commandContext.ExecutionMessage)
        {
            if (commandContext.Player is null)
                throw new MissingParameterException($"{commandContext.CommandIdentifier}", nameof(commandContext.Player));

            Player = commandContext.Player;
        }

        protected override async Task SendExecutionMessageAsync(string executionMessage)
        {

            await Player.GetStream().SendMessageAsync(executionMessage);
        }
    }
}