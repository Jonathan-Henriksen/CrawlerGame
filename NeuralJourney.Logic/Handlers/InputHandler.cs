using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Services;

namespace NeuralJourney.Logic.Handlers
{
    public class InputHandler : IInputHandler
    {
        private readonly IMessageService _messageService;

        public event Action<string>? OnAdminInputReceived;
        public event Action<string, Player>? OnPlayerInputReceived;

        public InputHandler(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task HandleAdminInputAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var input = await Console.In.ReadLineAsync(cancellationToken);

                if (string.IsNullOrEmpty(input))
                    continue;

                OnAdminInputReceived?.Invoke(input);
            }
        }

        public async Task HandlePlayerInputAsync(Player player, CancellationToken cancellationToken)
        {
            var stream = player.GetStream();
            while (!cancellationToken.IsCancellationRequested)
            {
                var input = await _messageService.ReadMessageAsync(stream, cancellationToken);
                if (_messageService.IsCloseConnectionMessage(input))
                {
                    return;
                }

                if (string.IsNullOrEmpty(input))
                    continue;

                OnPlayerInputReceived?.Invoke(input, player);
            }
        }
    }
}