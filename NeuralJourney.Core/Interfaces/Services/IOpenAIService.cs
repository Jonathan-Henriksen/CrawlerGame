using NeuralJourney.Core.Models.LogProperties;

namespace NeuralJourney.Core.Interfaces.Services
{
    public interface IOpenAIService
    {
        public Task<bool> SetCommandCompletionTextAsync(CommandContext context);
    }
}