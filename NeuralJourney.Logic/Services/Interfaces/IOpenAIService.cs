namespace NeuralJourney.Logic.Services.Interfaces
{
    public interface IOpenAIService
    {
        public Task<string> GetCommandCompletionTextAsync(string userinput);
    }
}