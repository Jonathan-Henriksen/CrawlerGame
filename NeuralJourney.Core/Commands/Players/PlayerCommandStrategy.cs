using NeuralJourney.Core.Exceptions;
using NeuralJourney.Core.Extensions;
using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Interfaces.Services;
using NeuralJourney.Core.Models.Commands;
using Serilog;

namespace NeuralJourney.Core.Commands.Players
{
    public class PlayerCommandStrategy : ICommandStrategy
    {
        private readonly ICommandMiddleware[] _middlewareProcessors;
        private readonly IMessageService _messageService;
        private readonly ILogger _logger;

        public PlayerCommandStrategy(IEnumerable<ICommandMiddleware> commandMiddleware, IMessageService messageService, ILogger logger)
        {
            _middlewareProcessors = commandMiddleware.ToArray();

            _messageService = messageService;

            _logger = logger.ForContext("Source", typeof(PlayerCommandStrategy).FullName);
        }

        public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken = default)
        {
            if (context.Player is null)
                throw new InvalidOperationException("Failed to execute player command strategy. Reason: Player was null");

            var errorMessage = string.Empty;

            try
            {
                var index = -1;

                async Task Next()
                {
                    if (++index < _middlewareProcessors.Length)
                        await _middlewareProcessors[index].InvokeAsync(context, Next, cancellationToken);
                }

                await Next();
            }
            catch (CommandCreationException ex)
            {
                _logger.Error(ex, "Failed to create command {@CommandContext}", context.ToSimplified());
                errorMessage = string.Format("Could not find any commands matching your input {0]", context.RawInput);
            }
            catch (CommandExecutionException ex)
            {
                _logger.Error(ex, "Error while executing command {@CommandContext}", context.ToSimplified());
                errorMessage = string.Format("Encountered an error while executing your command {0]", context.RawInput);
            }
            catch (OperationCanceledException)
            {
                return; // Do nothing on intended cancellation. Player Handler closes connections gracefully
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errorMessage) && context.Player is not null)
                    await _messageService.SendMessageAsync(context.Player.GetClient(), errorMessage, cancellationToken);
            }
        }
    }
}