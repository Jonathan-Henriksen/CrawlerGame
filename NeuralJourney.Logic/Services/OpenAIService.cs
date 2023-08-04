using NeuralJourney.Library.Attributes;
using NeuralJourney.Logic.Commands.Players.Base;
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
        private readonly string _availableCommands;

        public OpenAIService(OpenAIOptions options)
        {
            _openApi = new OpenAIAPI(new APIAuthentication(options.ApiKey));

            _openApi.Completions.DefaultCompletionRequestArgs.Model = options.Model;
            _openApi.Completions.DefaultCompletionRequestArgs.MaxTokens = options.MaxTokens;
            _openApi.Completions.DefaultCompletionRequestArgs.StopSequence = options.StopSequence;
            _openApi.Completions.DefaultCompletionRequestArgs.Temperature = options.Temperature;
            _openApi.Completions.DefaultCompletionRequestArgs.user = "Nerual Journey";

            _availableCommands = GetAvailableCommands();
        }

        public async Task<string> GetCommandCompletionAsync(string userinput)
        {
            var completionResponse = await _openApi.Completions.CreateCompletionAsync($"{_availableCommands}\n\n{userinput}\n\n###\n\n");
            return completionResponse?.Completions.FirstOrDefault()?.Text ?? string.Empty;
        }

        private static string GetAvailableCommands()
        {
            var stringBuilder = new StringBuilder();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var types = assemblies
                .SelectMany(assembly => assembly.GetExportedTypes())
                .Where(type => typeof(PlayerCommand).IsAssignableFrom(type) && type != typeof(PlayerCommand));

            foreach (var type in types)
            {
                var attribute = type.GetCustomAttribute<PlayerCommandMappingAttribute>();
                if (attribute is not null)
                {
                    var constructorParams = ExtractConstructorParameters(type);
                    stringBuilder.Append($"{attribute.Command}|{constructorParams},");
                }
            }

            return stringBuilder.ToString().TrimEnd(',');
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