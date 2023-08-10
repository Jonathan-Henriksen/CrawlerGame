namespace NeuralJourney.Logic.Services
{
    public interface IOpenAIService
    {
        public Task<string> GetCommandCompletionTextAsync(string userinput);
    }
}