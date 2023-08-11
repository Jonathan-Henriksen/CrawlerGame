namespace NeuralJourney.Core.Interfaces.Services
{
    public interface IOpenAIService
    {
        public Task<string> GetCommandCompletionTextAsync(string userinput);
    }
}