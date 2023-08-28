using NeuralJourney.Core.Commands;
using NeuralJourney.Core.Constants;
using NeuralJourney.Core.Enums.Commands;
using NeuralJourney.Core.Exceptions;
using NeuralJourney.Core.Interfaces.Services;
using NeuralJourney.Core.Models.LogProperties;
using NeuralJourney.Core.Models.Options;
using OpenAI_API;
using OpenAI_API.Completions;
using Serilog;
using System.Net;

namespace NeuralJourney.Infrastructure.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly ILogger _logger;
        private readonly OpenAIAPI _openApi;
        private readonly OpenAIOptions _options;

        private const int _maxRetryAttempts = 3;

        public OpenAIService(OpenAIOptions options, ILogger logger)
        {
            _options = options;
            _logger = logger.ForContext<OpenAIService>();

            _openApi = new OpenAIAPI(new APIAuthentication(options.ApiKey));

            _openApi.Completions.DefaultCompletionRequestArgs.Model = options.ParameterExtractionModel;
            _openApi.Completions.DefaultCompletionRequestArgs.MaxTokens = options.MaxTokens;
            _openApi.Completions.DefaultCompletionRequestArgs.StopSequence = options.StopSequence;
            _openApi.Completions.DefaultCompletionRequestArgs.Temperature = options.Temperature;
        }

        public async Task<CommandIdentifierEnum> GetCommandClassificationAsync(string inputText)
        {
            var request = new CompletionRequest(prompt: $"{inputText}\n\n###\n\n", max_tokens: 1, model: _options.CommandClassificationModel);

            var completionResult = await _openApi.Completions.CreateCompletionAsync(request);

            if (completionResult is null)
                throw new CommandMappingException("Command classification result was null");

            if (!completionResult.Completions.Any())
                throw new CommandMappingException("Command classification did not contain any completions");

            var completionText = completionResult.Completions.First().Text;

            _logger.Debug("Received completion text {CompletionText} for classification", completionText);

            if (!Enum.TryParse<CommandIdentifierEnum>(completionText, true, out var commandIndentifier))
                throw new CommandMappingException("Command classification was invalid");

            return commandIndentifier;
        }

        public async Task<bool> SetCommandCompletionTextAsync(CommandContext context)
        {
            var retryAttempts = 0;

            while (retryAttempts < _maxRetryAttempts)
            {
                var retryLogger = _logger.ForContext("RetryCount", retryAttempts++);

                try
                {
                    CommandContext? previousCommand = null;
                    context.Player?.PreviousCommands.TryPeek(out previousCommand);

                    var previousInput = string.Empty;

                    if (previousCommand is not null && (context.Player?.HasIncompleteCommand ?? false))
                        previousInput = $"{previousCommand.InputText}\n\n{previousCommand.CompletionText?.Split('|').LastOrDefault()}\n\n";

                    var promptText = $"{context.CommandKey.Identifier}|{previousInput}{context.InputText}\n\n###\n\n";


                    retryLogger.ForContext("PromptText", promptText.Replace("\n", "\\n")).Debug(CommandLogMessages.Debug.CompletionTextRequested, context.InputText);

                    var completionResult = await _openApi.Completions.CreateCompletionAsync(promptText);
                    context.CompletionText = completionResult?.Completions.FirstOrDefault()?.Text.TrimStart();

                    retryLogger.ForContext("CompletionResult", completionResult, true)
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
    }
}