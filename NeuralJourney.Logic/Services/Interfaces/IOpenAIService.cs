using NeuralJourney.Library.Models.OpenAI;

namespace NeuralJourney.Logic.Services.Interfaces
{
    public interface IOpenAIService
    {
        public Task<CommandInfo> GetCommandFromPlayerInput(string userinput, IEnumerable<string> commands);

        public void Init();
    }
}