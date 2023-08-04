using NeuralJourney.Library.Enums;
using NeuralJourney.Library.Models.CommandInfo;
using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Commands.Admin.Base;
using NeuralJourney.Logic.Commands.Players.Base;
using NeuralJourney.Logic.Dispatchers.Interfaces;
using NeuralJourney.Logic.Factories.Interfaces;
using NeuralJourney.Logic.Services.Interfaces;

namespace NeuralJourney.Logic.Dispatchers
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IOpenAIService _openAIService;

        private readonly ICommandFactory<AdminCommand, AdminCommandInfo> _adminCommandFactory;
        private readonly ICommandFactory<PlayerCommand, PlayerCommandInfo> _playerCommandFactory;

        public CommandDispatcher(IOpenAIService openAIService, ICommandFactory<AdminCommand, AdminCommandInfo> adminCommandFactory, ICommandFactory<PlayerCommand, PlayerCommandInfo> playerCommandFactory)
        {
            _openAIService = openAIService;
            _adminCommandFactory = adminCommandFactory;
            _playerCommandFactory = playerCommandFactory;
        }

        public async void DispatchAdminCommandAsync(string input)
        {
            var commandInfo = new AdminCommandInfo(AdminCommandEnum.Unknown, Array.Empty<string>(), input, string.Empty);
            var command = _adminCommandFactory.GetCommand(commandInfo);

            if (command is null)
                return;

            await command.ExecuteAsync();
        }

        public async void DispatchPlayerCommandAsync((Player Player, string Text) input)
        {
            var completion = await _openAIService.GetCommandCompletionAsync(input.Text);
            var commandInfo = GetCommandInfoFromCompletion(input.Player, completion);

            var command = _playerCommandFactory.GetCommand(commandInfo);

            if (command is null)
                return;

            await command.ExecuteAsync();
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