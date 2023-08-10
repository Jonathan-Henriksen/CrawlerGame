using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Handlers.Interfaces;
using NeuralJourney.Logic.Services.Interfaces;

namespace NeuralJourney.Logic.Handlers
{
    public class PlayerInputHandler : IInputHandler<Player>
    {
        private readonly IMessageService _messageService;

        public event Action<string, Player>? OnInputReceived;

        public PlayerInputHandler(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task HandleInputAsync(Player player, CancellationToken cancellationToken = default)
        {
            if (player is null)
                throw new InvalidOperationException("Cannot handle player input. Reason: Player was null");

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

                OnInputReceived?.Invoke(input, player);
            }
        }
    }
}