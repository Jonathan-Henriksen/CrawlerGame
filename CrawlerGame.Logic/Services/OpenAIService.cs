using CrawlerGame.Library.Enums;
using CrawlerGame.Library.Models.ChatGPT;
using CrawlerGame.Logic.Options;
using CrawlerGame.Logic.Services.Interfaces;
using OpenAI_API;

namespace CrawlerGame.Logic.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly OpenAIAPI _openApi;
        private readonly string? _model;

        public OpenAIService(OpenAIOptions options)
        {
            _openApi = new OpenAIAPI(new APIAuthentication(options.ApiKey));

            _openApi.Completions.DefaultCompletionRequestArgs.Model = options.Model;
            _openApi.Completions.DefaultCompletionRequestArgs.Suffix = options.CompletionSuffix;
        }

        public async Task<CommandInfo> GetCommandFromPlayerInput(string userinput, IEnumerable<string> availableCommands)
        {

            var completionResponse = await _openApi.Completions.CreateCompletionAsync($"{string.Join(',', availableCommands)}\n\n{userinput}\n\n###\n\n");
            var completion = completionResponse?.Completions.FirstOrDefault();

            if (completion is null)
                return new CommandInfo() { Command = CommandEnum.Unknown };

            var completionText = completion.Text.Split(',').ToDictionary(d => d.Split(':')[0], d => d.Split(':')[1]);

            return new CommandInfo()
            {
                Command = (CommandEnum) Enum.Parse(typeof(CommandEnum), completionText["Command"]),
                SuccessMessage = completionText["SuccessMessage"],
                Param = completionText["Param"]
            };
        }
    }
}