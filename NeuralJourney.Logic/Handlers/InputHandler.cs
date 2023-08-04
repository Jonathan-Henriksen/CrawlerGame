using NeuralJourney.Library.Models.World;
using NeuralJourney.Library.Extensions;
using NeuralJourney.Logic.Handlers.Interfaces;
using NeuralJourney.Logic.Services.Interfaces;
using NeuralJourney.Library.Enums;
using NeuralJourney.Library.Models.CommandInfo;
using System.Net.Sockets;

namespace NeuralJourney.Logic.Handlers
{
    public class InputHandler : IInputHandler
    {
        private readonly IOpenAIService _openAIService;

        public event Action<AdminCommandInfo>? OnAdminInputReceived;
        public event Action<PlayerCommandInfo>? OnPlayerInputReceived;

        public InputHandler(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        public async Task HandleAdminInputAsync()
        {
            while (true)
            {
                var input = await Console.In.ReadLineAsync();

                if (string.IsNullOrEmpty(input))
                    continue;

                var commandInfo = new AdminCommandInfo(AdminCommandEnum.Unknown, Array.Empty<string>(), input, string.Empty);

                OnAdminInputReceived?.Invoke(commandInfo);
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

                var completionText = await _openAIService.GetCommandCompletionTextAsync(input);
                var commandInfo = GetCommandInfoFromCompletionText(player, completionText);

                OnPlayerInputReceived?.Invoke(commandInfo);
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

        private static PlayerCommandInfo GetCommandInfoFromCompletionText(Player player, string completionText)
        {
            var mainParts = completionText.Split('|');

            var commandInfo = new PlayerCommandInfo(player, PlayerCommandEnum.Unknown, Array.Empty<string>(), string.Empty, string.Empty);

            if (mainParts.Length > 0)
            {
                var commandParts = mainParts[0].Split(new[] { "(", ")" }, StringSplitOptions.None) ?? Array.Empty<string>();

                var commandText = commandParts[0].TrimStart();
                if (Enum.TryParse(commandText, true, out PlayerCommandEnum cmd))
                {
                    commandInfo.PlayerCommand = cmd;
                }

                if (commandParts.Length > 1)
                    commandInfo.Params = commandParts[1].Split(',');
            }

            if (mainParts.Length > 1)
            {
                commandInfo.SuccessMessage = mainParts[1];
            }

            return commandInfo;
        }
    }
}