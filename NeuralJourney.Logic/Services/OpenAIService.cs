using NeuralJourney.Library.Enums.Commands;
using NeuralJourney.Library.Exceptions.Commands;
using NeuralJourney.Logic.Commands;
using NeuralJourney.Logic.Options;
using NeuralJourney.Logic.Services.Interfaces;
using OpenAI_API;

namespace NeuralJourney.Logic.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly OpenAIAPI _openApi;

        private readonly string _availableCommands;

        public OpenAIService(OpenAIOptions options)
        {
            _openApi = new OpenAIAPI(new APIAuthentication(options.ApiKey));

            _openApi.Completions.DefaultCompletionRequestArgs.Model = options.Model;
            _openApi.Completions.DefaultCompletionRequestArgs.MaxTokens = options.MaxTokens;
            _openApi.Completions.DefaultCompletionRequestArgs.StopSequence = options.StopSequence;
            _openApi.Completions.DefaultCompletionRequestArgs.Temperature = options.Temperature;

            _availableCommands = CommandRegistry.GetCommands(CommandTypeEnum.Player);
        }

        public async Task<string> GetCommandCompletionTextAsync(string userinput)
        {
            var completionResponse = await _openApi.Completions.CreateCompletionAsync($"{_availableCommands}\n\n{userinput}\n\n###\n\n");

            var completionText = completionResponse?.Completions.FirstOrDefault()?.Text;

            if (string.IsNullOrEmpty(completionText))
                throw new InvalidCompletionTextException(completionText, "Completion text was empty");

            return completionText;
        }
    }
}