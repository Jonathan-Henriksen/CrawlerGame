using NeuralJourney.Library.Extensions;
using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Handlers.Interfaces;

namespace NeuralJourney.Logic.Handlers
{
    public class InputHandler : IInputHandler
    {
        public event Action<string>? OnAdminInputReceived;
        public event Action<(Player Player, string PlayerInput)>? OnPlayerInputReceived;

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
            using var stream = player.GetStream();
            while (player.IsConnected)
            {
                if (!stream.DataAvailable)
                {
                    await Task.Delay(100);
                    continue;
                }

                var input = await stream.ReadMessageAsync();

                if (string.IsNullOrEmpty(input))
                    continue;

                OnPlayerInputReceived?.Invoke((player, input));
            }
        }
    }
}