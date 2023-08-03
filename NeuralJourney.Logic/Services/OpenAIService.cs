using NeuralJourney.Library.Attributes;
using NeuralJourney.Library.Enums;
using NeuralJourney.Library.Models.ChatGPT;
using NeuralJourney.Logic.Commands.Base;
using NeuralJourney.Logic.Options;
using NeuralJourney.Logic.Services.Interfaces;
using OpenAI_API;
using System.Reflection;
using System.Text;

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

        public void Init()
        {
            var stringBuilder = new StringBuilder();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var types = assemblies
                .SelectMany(assembly => assembly.GetExportedTypes())
                .Where(type => typeof(Command).IsAssignableFrom(type) && type != typeof(Command));

            foreach (var type in types)
            {
                var attribute = type.GetCustomAttribute<CommandMappingAttribute>();
                if (attribute != null)
                {
                    var constructorParams = ExtractConstructorParameters(type);
                    stringBuilder.Append($"{attribute.Command}|{constructorParams},");
                }
            }

            var typeMappingsString = stringBuilder.ToString().TrimEnd(',');

            Console.WriteLine(typeMappingsString);
        }

        private static string ExtractConstructorParameters(Type type)
        {
            var constructors = type.GetConstructors()
                .Where(c => c.DeclaringType == type)
                .ToArray();

            List<string> constructorParamsList = constructors
                .Select(constructor => string.Join("", Enumerable.Range(0, constructor.GetParameters().Length).Select(i => $"{{{i}}}")))
                .ToList();

            return string.Join("|", constructorParamsList);
        }
    }
}