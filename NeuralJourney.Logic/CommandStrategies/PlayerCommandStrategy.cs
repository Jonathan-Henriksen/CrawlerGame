using NeuralJourney.Library.Enums.Commands;
using NeuralJourney.Library.Exceptions.Commands;
using NeuralJourney.Library.Exceptions.Commands.Base;
using NeuralJourney.Library.Exceptions.PlayerActions.Base;
using NeuralJourney.Library.Models.CommandInfo;
using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Commands.Players.Base;
using NeuralJourney.Logic.CommandStrategies.Interfaces;
using NeuralJourney.Logic.Factories.Interfaces;
using NeuralJourney.Logic.Services.Interfaces;

namespace NeuralJourney.Logic.CommandStrategies
{
    public class PlayerCommandStrategy : IPlayerCommandStrategy
    {
        private readonly ICommandFactory<PlayerCommand, PlayerCommandEnum> _commandFactory;
        private readonly IOpenAIService _openAIService;

        public PlayerCommandStrategy(ICommandFactory<PlayerCommand, PlayerCommandEnum> commandFactory, IOpenAIService openAIService)
        {
            _commandFactory = commandFactory;
            _openAIService = openAIService;
        }

        public async Task ExecuteAsync(string playerInput, Player player)
        {
            try
            {
                var completionText = await _openAIService.GetCommandCompletionTextAsync(playerInput);

                var commandInfo = GetCommandInfoFromCompletionText(completionText, player);

                var command = _commandFactory.CreateCommand(commandInfo);

                await command.ExecuteAsync();
            }
            catch (InvalidCommandException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (MissingParameterException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (InvalidParameterException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (CommandMappingException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (PlayerActionException ex)
            {
                Console.WriteLine(ex.Message);
            }
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