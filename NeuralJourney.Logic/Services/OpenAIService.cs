using NeuralJourney.Library.Attributes;
using NeuralJourney.Library.Exceptions.Commands;
using NeuralJourney.Logic.Commands.Players;
using NeuralJourney.Logic.Options;
using OpenAI_API;
using System.Reflection;
using System.Text;

namespace NeuralJourney.Logic.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly OpenAIAPI _openApi;

        private readonly string AvailableCommands;

        public OpenAIService(OpenAIOptions options)
        {
            _openApi = new OpenAIAPI(new APIAuthentication(options.ApiKey));

            _openApi.Completions.DefaultCompletionRequestArgs.Model = options.Model;
            _openApi.Completions.DefaultCompletionRequestArgs.MaxTokens = options.MaxTokens;
            _openApi.Completions.DefaultCompletionRequestArgs.StopSequence = options.StopSequence;
            _openApi.Completions.DefaultCompletionRequestArgs.Temperature = options.Temperature;

            AvailableCommands = GetAvailableCommands();
        }

        public async Task<string> GetCommandCompletionTextAsync(string userinput)
        {
            var completionResponse = await _openApi.Completions.CreateCompletionAsync($"{AvailableCommands}\n\n{userinput}\n\n###\n\n");

            var completionText = completionResponse?.Completions.FirstOrDefault()?.Text;

            if (string.IsNullOrEmpty(completionText))
                throw new InvalidCompletionDataException(completionText, string.Format("Completion text for the user input \'{0}\' was null or empty", userinput));

            return completionText;
        }

        private static string GetAvailableCommands()
        {
            var stringBuilder = new StringBuilder();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var types = assemblies
                .SelectMany(assembly => assembly.GetExportedTypes())
                .Where(type => typeof(PlayerCommandBase).IsAssignableFrom(type) && type != typeof(PlayerCommandBase));

            foreach (var type in types)
            {
                var attribute = type.GetCustomAttribute<CommandTypeAttribute>();
                if (attribute is not null)
                {
                    var constructorParams = ExtractConstructorParameters(type);
                    stringBuilder.Append($"{attribute.CommandType}|{constructorParams},");
                }
            }

            return stringBuilder.ToString().TrimEnd(',');
        }

        private static string ExtractConstructorParameters(Type type)
        {
            var constructors = type.GetConstructors()
                .Where(c => c.DeclaringType == type)
                .ToArray();

            var constructorParamsList = constructors
                .Select(constructor => string.Join("", Enumerable.Range(0, constructor.GetParameters().Length).Select(i => $"{{{i}}}")))
                .ToList();

            return string.Join("|", constructorParamsList);
        }
    }
}