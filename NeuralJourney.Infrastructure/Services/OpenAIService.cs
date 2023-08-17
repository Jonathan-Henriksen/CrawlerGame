using NeuralJourney.Core.Commands;
using NeuralJourney.Core.Constants;
using NeuralJourney.Core.Enums.Commands;
using NeuralJourney.Core.Interfaces.Services;
using NeuralJourney.Core.Models.Commands;
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

        private readonly string _availableCommands;

        private const int _maxRetryAttempts = 3;
        private int RetryCount = 0;

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
            while (RetryCount < _maxRetryAttempts)
            {
                var retryLogger = _logger.ForContext("RetryCount", RetryCount++);

                try
                {
                    var promptText = $"{_availableCommands}\n\n{context.InputText}\n\n###\n\n";

                    retryLogger.Debug(CommandLogMessages.Debug.CompletionTextRequested, context.InputText);

                    var completionResponse = await _openApi.Completions.CreateCompletionAsync(promptText);
                    context.CompletionText = completionResponse?.Completions.FirstOrDefault()?.Text.TrimStart();

                    retryLogger.ForContext("CompletionResponse", completionResponse, true)
                        .Debug(CommandLogMessages.Debug.CompletionTextReceived, context.CompletionText);

                    return true;
                }
                catch (WebException webEx) when (webEx.Status == WebExceptionStatus.Timeout || webEx.Status == WebExceptionStatus.ConnectFailure)
                {
                    retryLogger.Warning(webEx, NetworkLogMessages.Warning.OpenAIRequestTimeout);

                    await Task.Delay(1000 * RetryCount);
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