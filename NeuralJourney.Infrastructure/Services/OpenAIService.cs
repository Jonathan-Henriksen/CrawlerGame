using NeuralJourney.Core.Commands;
using NeuralJourney.Core.Enums.Commands;
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
                try
                {
                    var promptText = $"{_availableCommands}\n\n{context.RawInput}\n\n###\n\n";

                    _logger.Debug("Requesting completion from OpenAI {@CompletionRequest}", new { context.RawInput, promptText });

                    var completionResponse = await _openApi.Completions.CreateCompletionAsync(promptText);
                    var completionText = completionResponse?.Completions.FirstOrDefault()?.Text;

                    var completionData = new
                    {
                        completionResponse?.Id,
                        completionResponse?.RequestId,
                        TokenUsage = completionResponse?.Usage.TotalTokens,
                        completionResponse?.ProcessingTime,
                        Completion = completionResponse?.Completions.FirstOrDefault(),
                        RetryCount = retryCount
                    };

                    _logger.Debug("Received completion from OpenAI {@CompletionResponse}", completionData);

                    if (string.IsNullOrEmpty(completionText))
                        throw new InvalidOperationException("Completion text was empty");

                    return completionText;
                }
                catch (WebException webEx) when (webEx.Status == WebExceptionStatus.Timeout || webEx.Status == WebExceptionStatus.ConnectFailure)
                {
                    var retryData = new { Reason = webEx.Status, RetryCount = retryCount };
                    _logger.Warning(webEx, "Network error while requesting OpenAI completion {@CompletionRetryData}", retryData);

                    retryCount++;

                    await Task.Delay(1000 * retryCount);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            throw new InvalidOperationException("Retry limit exceeded");
        }

    }
}