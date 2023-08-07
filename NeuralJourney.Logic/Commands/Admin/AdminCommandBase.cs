using NeuralJourney.Library.Attributes;
using NeuralJourney.Library.Enums.Commands;
using NeuralJourney.Library.Models.CommandContext;

namespace NeuralJourney.Logic.Commands.Admin
{
    [CommandType(CommandTypeEnum.Admin)]
    public abstract class AdminCommandBase : CommandBase
    {
        protected AdminCommandBase(CommandContext commandInfo) : base(commandInfo.Params, commandInfo.ExecutionMessage)
        {
        }

        protected override Task SendExecutionMessageAsync(string responseMessage)
        {
            Console.WriteLine(responseMessage);
            return Task.CompletedTask;
        }
    }
}