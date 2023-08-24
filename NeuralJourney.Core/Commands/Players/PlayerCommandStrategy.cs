using NeuralJourney.Core.Exceptions;
using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Interfaces.Services;
using NeuralJourney.Core.Models.LogProperties;
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

            _logger = logger.ForContext<PlayerCommandStrategy>();
        }

        public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken = default)
        {
            var errorMessage = string.Empty;

            try
            {
                if (context.Player is null)
                    throw new InvalidOperationException("Player was null");

                var index = -1;

                async Task Next()
                {
                    if (++index < _middlewareProcessors.Length)
                        await _middlewareProcessors[index].InvokeAsync(context, Next, cancellationToken);
                }

                await Next();

                if (context.Result.HasValue && context.Result.Value.Success)
                    context.Player.HasIncompleteCommand = false;
            }
            catch (Exception ex) when (ex is CommandMappingException || ex.InnerException is CommandMappingException)
            {
                var mappingEx = ex as CommandMappingException ?? ex.InnerException as CommandMappingException;

                _logger.Error(mappingEx, "Failed to map input to command");
                errorMessage = mappingEx?.PlayerMessage;
            }
            catch (Exception ex) when (ex is CommandExecutionException || ex.InnerException is CommandExecutionException)
            {
                var executiionEx = ex as CommandExecutionException ?? ex.InnerException as CommandExecutionException;

                _logger.Error(executiionEx, "Error while executing command");
                errorMessage = executiionEx?.PlayerMessage;
            }
            catch (OperationCanceledException)
            {
                return; // Do nothing on intended cancellation. Player Handler closes connections gracefully
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected error while processing the command");
                errorMessage = "An unexpected error occured. Please try again";
            }
            finally
            {
                if (!string.IsNullOrEmpty(errorMessage) && context.Player is not null)
                    await _messageService.SendMessageAsync(context.Player.Client, errorMessage, cancellationToken);
                else
                    _logger.Debug("Successfully executed command {CommandIdentifier} for player {PlayerName}", context.CommandKey.Identifier, context.Player?.Name);

                context.Player?.PreviousCommands.Push(context);
            }
        }
    }
}