using NeuralJourney.Core.Exceptions.Commands;
using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Interfaces.Services;
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
            _logger = logger;
        }

        public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken = default)
        {
            if (context.Player is null)
                throw new InvalidOperationException("Cannot execute player command strategy. Reason: Player was null");

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
            catch (InvalidCompletionTextException ex)
            {
                _logger.Error(ex, ex.Message);
                errorMessage = ex.Message;
            }
            catch (InvalidCommandException ex)
            {
                _logger.Error(ex, ex.Message);
                errorMessage = ex.Message;
            }
            catch (MissingParameterException ex)
            {
                _logger.Error(ex, ex.Message);
                errorMessage = ex.Message;
            }
            catch (InvalidParameterException ex)
            {
                _logger.Error(ex, ex.Message);
                errorMessage = ex.Message;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errorMessage) && context.Player is not null)
                    await _messageService.SendMessageAsync(context.Player.GetStream(), errorMessage, cancellationToken);
            }
        }
    }
}