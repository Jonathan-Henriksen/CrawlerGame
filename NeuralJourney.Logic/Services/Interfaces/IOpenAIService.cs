using NeuralJourney.Library.Models.ChatGPT;

namespace NeuralJourney.Logic.Services.Interfaces
{
    public interface IOpenAIService
    {
        public Task<CommandInfo> GetCommandFromPlayerInput(string userinput, IEnumerable<string> commands);

        public void Init();
    }
}