using NeuralJourney.Library.Enums;
using NeuralJourney.Library.Models.ChatGPT;
using NeuralJourney.Logic.Options;
using NeuralJourney.Logic.Services.Interfaces;
using OpenAI_API;

namespace NeuralJourney.Logic.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly OpenAIAPI _openApi;

        public OpenAIService(OpenAIOptions options)
        {
            _openApi = new OpenAIAPI(new APIAuthentication(options.ApiKey));

            _openApi.Completions.DefaultCompletionRequestArgs.Model = options.Model;
            _openApi.Completions.DefaultCompletionRequestArgs.MaxTokens = options.MaxTokens;
            _openApi.Completions.DefaultCompletionRequestArgs.StopSequence = options.StopSequence;
            _openApi.Completions.DefaultCompletionRequestArgs.Temperature = options.Temperature;
            _openApi.Completions.DefaultCompletionRequestArgs.user = "Nerual Journey";
        }

        public async Task<CommandInfo> GetCommandFromPlayerInput(string userinput, IEnumerable<string> availableCommands)
        {
            var completionResponse = await _openApi.Completions.CreateCompletionAsync($"{string.Join(',', availableCommands)}\n\n{userinput}\n\n###\n\n");
            var completion = completionResponse?.Completions.FirstOrDefault();

            if (completion is null || string.IsNullOrEmpty(completion.Text))
                return new CommandInfo(CommandEnum.Unknown, userinput);

            return GetCommandInfoFromCompletion(completion.Text);
        }

        private CommandInfo GetCommandInfoFromCompletion(string completionText)
        {
            string[] mainParts = completionText.Split(new string[] { ",Param:", ",SuccessMessage:" }, StringSplitOptions.None);

            var commandInfo = new CommandInfo(CommandEnum.Unknown, Array.Empty<string>(), string.Empty);

            if (Enum.TryParse(mainParts[0].TrimStart(), true, out CommandEnum cmd))
            {
                commandInfo.Command = cmd;
            }

            if (mainParts.Length > 1 && mainParts[1] != "null")
            {
                commandInfo.Params = mainParts[1].Split(',');
            }

            if (mainParts.Length > 2 && mainParts[2] != "null")
            {
                commandInfo.SuccessMessage = mainParts[2];
            }

            return commandInfo;
        }
    }
}