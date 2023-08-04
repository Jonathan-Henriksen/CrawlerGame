using NeuralJourney.Library.Models.World;
using NeuralJourney.Library.Extensions;
using NeuralJourney.Logic.Services.Interfaces;
using NeuralJourney.Logic.Dispatchers.Interfaces;
using NeuralJourney.Logic.Handlers.Interfaces;
using NeuralJourney.Library.Enums;
using System.Net.Sockets;
using NeuralJourney.Library.Models.CommandInfo;

namespace NeuralJourney.Logic.Handlers
{
    public class InputHandler : IInputHandler
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IOpenAIService _openAIService;

        public InputHandler(ICommandDispatcher commandDispatcher, IOpenAIService openAIService)
        {
            _commandDispatcher = commandDispatcher;
            _openAIService = openAIService;
        }

        public async Task HandleAdminInputAsync()
        {
            while (true)
            {
                var adminInput = await Console.In.ReadLineAsync();

                if (string.IsNullOrEmpty(adminInput))
                    continue;

                var commandInfo = new AdminCommandInfo(AdminCommandEnum.Unknown, Array.Empty<string>(), adminInput, string.Empty);

                _ = _commandDispatcher.DispatchAdminCommandAsync(commandInfo);
            }
        }

        public async Task HandlePlayerInputAsync(Player player)
        {
            var stream = player.GetStream();
            while (player.IsConnected)
            {
                var playerInput = await GetPlayerInputAsync(stream, player);

                if (string.IsNullOrEmpty(playerInput))
                    continue;

                var completion = await _openAIService.GetCommandCompletionAsync(playerInput);
                var commandInfo = GetCommandInfoFromCompletion(player, completion);

                await _commandDispatcher.DispatchPlayerCommandAsync(commandInfo);
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

        private static PlayerCommandInfo GetCommandInfoFromCompletion(Player player, string completionText)
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