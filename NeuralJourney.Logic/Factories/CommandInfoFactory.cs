using NeuralJourney.Library.Enums;
using NeuralJourney.Library.Exceptions.Commands;
using NeuralJourney.Library.Models.CommandInfo;
using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Factories.Interfaces;
using NeuralJourney.Logic.Services.Interfaces;

namespace NeuralJourney.Logic.Factories
{
    public class CommandInfoFactory : ICommandInfoFactory
    {
        private readonly IOpenAIService _openAIService;

        public CommandInfoFactory(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        public async Task<AdminCommandInfo> CreateAdminCommandInfoFromInputAsync(string input)
        {
            return await Task.FromResult(new AdminCommandInfo(AdminCommandEnum.Announce, Array.Empty<string>(), input));
        }

        public async Task<PlayerCommandInfo> CreatePlayerCommandInfoFromInputAsync(string input, Player player)
        {
            var completionText = await _openAIService.GetCommandCompletionTextAsync(input);

            return GetCommandInfoFromCompletionText(completionText, player);
        }

        private static PlayerCommandInfo GetCommandInfoFromCompletionText(string completionText, Player player)
        {
            var mainParts = completionText.Split('|');

            if (mainParts.Length <= 1)
                throw new InvalidCompletionDataException(completionText, "Could not split text into Command and SuccessMessage");

            var commandParts = mainParts[0].Split(new[] { "(", ")" }, StringSplitOptions.None) ?? Array.Empty<string>();

            var commandName = commandParts[0].TrimStart();
            var commandParams = Array.Empty<string>();

            if (!Enum.TryParse(commandName, true, out PlayerCommandEnum commandEnum))
                throw new InvalidCompletionDataException(completionText, string.Format("Could not parse the command \'{0}\' to {1}", commandName, nameof(PlayerCommandEnum)));

            if (commandParts.Length >= 2)
                commandParams = commandParts[1].Split(',');

            if (commandParams.Any(string.IsNullOrEmpty))
                throw new InvalidCompletionDataException(completionText, "Has empty command parameters");

            var successMessage = mainParts[1];

            if (string.IsNullOrEmpty(successMessage))
                throw new InvalidCompletionDataException(completionText, "SuccessMessage is blank");

            return new PlayerCommandInfo(commandEnum, commandParams, successMessage, player);
        }
    }
}