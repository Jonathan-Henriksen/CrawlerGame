using NeuralJourney.Core.Commands;
using NeuralJourney.Core.Constants;
using NeuralJourney.Core.Enums.Commands;
using NeuralJourney.Core.Interfaces.Services;
using NeuralJourney.Core.Models.LogProperties;
using NeuralJourney.Core.Models.Options;
using OpenAI_API;
using Serilog;
using System.Net;

namespace NeuralJourney.Infrastructure.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly ILogger _logger;
        private readonly OpenAIAPI _openApi;

        private readonly Dictionary<string, int> _availableCommands;

        private const int _maxRetryAttempts = 3;

        public OpenAIService(OpenAIOptions options, ILogger logger)
        {
            _logger = logger.ForContext<OpenAIService>();

            _openApi = new OpenAIAPI(new APIAuthentication(options.ApiKey));

            _openApi.Completions.DefaultCompletionRequestArgs.Model = options.Model;
            _openApi.Completions.DefaultCompletionRequestArgs.MaxTokens = options.MaxTokens;
            _openApi.Completions.DefaultCompletionRequestArgs.StopSequence = options.StopSequence;
            _openApi.Completions.DefaultCompletionRequestArgs.Temperature = options.Temperature;

            _availableCommands = CommandRegistry.GetCommands(CommandTypeEnum.Player);
        }

        public async Task<bool> SetCommandCompletionTextAsync(CommandContext context)
        {
            var retryAttempts = 0;

            while (retryAttempts < _maxRetryAttempts)
            {
                var retryLogger = _logger.ForContext("RetryCount", retryAttempts++);

                try
                {
                    var promptText = GeneratePrompt(context.InputText);

                    retryLogger.ForContext("PromptText", promptText.Replace("\n", "\\n")).Debug(CommandLogMessages.Debug.CompletionTextRequested, context.InputText);

                    var completionResponse = await _openApi.Completions.CreateCompletionAsync(promptText);
                    context.CompletionText = completionResponse?.Completions.FirstOrDefault()?.Text.TrimStart();

                    retryLogger.ForContext("CompletionResponse", completionResponse, true)
                        .Debug(CommandLogMessages.Debug.CompletionTextReceived, context.CompletionText);

                    return true;
                }
                catch (WebException webEx) when (webEx.Status == WebExceptionStatus.Timeout || webEx.Status == WebExceptionStatus.ConnectFailure)
                {
                    retryLogger.Warning(webEx, NetworkLogMessages.Warning.OpenAIRequestTimeout);

                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retryAttempts))); // Exponential back-off
                }
                catch (Exception ex)
                {
                    retryLogger.Error(ex, CommandLogMessages.Error.CompletionTextRequstFailed);
                    throw;
                }
            }

            return false;
        }

        private string GeneratePrompt(string inputText)
        {
            var formattedCommands = string.Join(",", _availableCommands.Select(command =>
            {
                if (command.Value < 1)
                    return command.Key;

                var parameters = string.Join("", Enumerable.Range(0, command.Value).Select(i => $"{{{i}}}"));

                return $"{command.Key}{parameters}";

            }));

            return $"{formattedCommands}\n\n{inputText}\n\n###\n\n";
        }
    }
}