using NeuralJourney.Core.Commands;
using NeuralJourney.Core.Enums.Commands;
using NeuralJourney.Core.Exceptions;
using NeuralJourney.Core.Interfaces.Services;
using NeuralJourney.Core.Models.Commands;
using NeuralJourney.Core.Options;
using OpenAI_API;
using Serilog;
using System.Net;

namespace NeuralJourney.Infrastructure.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly ILogger _logger;
        private readonly OpenAIAPI _openApi;

        private readonly string _availableCommands;

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

        public async Task<string> GetCommandCompletionTextAsync(CommandContext context)
        {
            var retryCount = 0;

            while (retryCount < _maxRetryAttempts)
            {
                var retryLogger = _logger.ForContext("RetryCount", retryCount++);

                try
                {
                    var promptText = $"{_availableCommands}\n\n{context.RawInput}\n\n###\n\n";

                    retryLogger.Debug("Requesting completion from OpenAI '{PromptText}'", promptText.Replace("\n", "\\n"));

                    var completionResponse = await _openApi.Completions.CreateCompletionAsync(promptText);
                    var completionText = completionResponse?.Completions.FirstOrDefault()?.Text.TrimStart();

                    retryLogger.ForContext("CompletionResponse", completionResponse, true)
                        .Debug("Received completion from OpenAI '{CompletionText}'", completionText);

                    if (string.IsNullOrEmpty(completionText))
                        throw new CommandMappingException("Completion text was empty", "The game encountered an error with the OpenAI API. Please try agian");

                    return completionText;
                }
                catch (WebException webEx) when (webEx.Status == WebExceptionStatus.Timeout || webEx.Status == WebExceptionStatus.ConnectFailure)
                {
                    retryLogger.Warning(webEx, "Network error while requesting OpenAI completion");

                    await Task.Delay(1000 * retryCount);
                }
                catch (Exception ex)
                {
                    retryLogger.Error(ex, "Unexpected error while requestion OpenAI completion");
                    throw;
                }
            }

            throw new CommandMappingException("Failed to retrieve OpenAI completion", "The server could not connect to the OpenAI API. Please try agian.");
        }
    }
}