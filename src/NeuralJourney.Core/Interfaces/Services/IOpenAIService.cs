using NeuralJourney.Core.Enums.Commands;
using NeuralJourney.Core.Models.LogProperties;

namespace NeuralJourney.Core.Interfaces.Services
{
    public interface IOpenAIService
    {
        Task<CommandIdentifierEnum> GetCommandClassificationAsync(string inputText);

        Task<bool> SetCommandCompletionTextAsync(CommandContext context);
    }
}