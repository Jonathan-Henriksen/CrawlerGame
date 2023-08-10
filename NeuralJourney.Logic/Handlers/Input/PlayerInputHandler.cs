using NeuralJourney.Library.Enums.Commands;
using NeuralJourney.Library.Models.Commands;
using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Services.Interfaces;

namespace NeuralJourney.Logic.Handlers.Input
{
    public class PlayerInputHandler : IInputHandler
    {
        private readonly IMessageService _messageService;

        public event Action<CommandContext>? OnInputReceived;

        public PlayerInputHandler(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task HandleInputAsync(Player? player = default, CancellationToken cancellationToken = default)
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

                var context = new CommandContext(input, CommandTypeEnum.Player, player);

                OnInputReceived?.Invoke(context);
            }
        }
    }
}