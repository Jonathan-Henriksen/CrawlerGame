namespace NeuralJourney.Logic.Services.Interfaces
{
    public interface IOpenAIService
    {
        public Task<string> GetCommandCompletionAsync(string userinput);
    }
}