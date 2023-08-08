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

        public async Task HandleAdminInputAsync()
        {
            while (true)
            {
                var input = await Console.In.ReadLineAsync();

                if (string.IsNullOrEmpty(input))
                    continue;

                OnAdminInputReceived?.Invoke(input);
            }
        }

        public async Task HandlePlayerInputAsync(Player player)
        {
            var stream = player.GetStream();
            while (player.IsConnected)
            {
                var input = await _messageService.ReadMessageAsync(stream);
                if (_messageService.IsCloseConnectionMessage(input))
                {
                    // TODO: Add cancellation token logic to replace IsConnected
                    return;
                }

                if (string.IsNullOrEmpty(input))
                    continue;

                OnPlayerInputReceived?.Invoke(input, player);
            }
        }
    }
}