using NeuralJourney.Core.Models.Commands;

namespace NeuralJourney.Core.Interfaces.Services
{
    public interface IOpenAIService
    {
        public Task SetCommandCompletionTextAsync(CommandContext context);
    }
}