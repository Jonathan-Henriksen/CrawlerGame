using NeuralJourney.Library.Models.World;
using NeuralJourney.Library.Extensions;
using NeuralJourney.Logic.Handlers.Interfaces;
using System.Net.Sockets;

namespace NeuralJourney.Logic.Handlers
{
    public class InputHandler : IInputHandler
    {
        public event Action<string>? OnAdminInputReceived;
        public event Action<(Player, string)>? OnPlayerInputReceived;

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
                var input = await GetPlayerInputAsync(stream, player);

                if (string.IsNullOrEmpty(input))
                    continue;

                OnPlayerInputReceived?.Invoke((player, input));
            }
        }

        private static async Task<string?> GetPlayerInputAsync(NetworkStream stream, Player player)
        {
            while (player.IsConnected)
            {
                if (!stream.DataAvailable)
                    await Task.Delay(100);
                else
                    return await stream.ReadMessageAsync();
            }

            return default;
        }
    }
}