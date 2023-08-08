using NeuralJourney.Library.Enums.Commands;
using NeuralJourney.Library.Exceptions.Commands;
using NeuralJourney.Library.Exceptions.PlayerActions.Base;
using NeuralJourney.Library.Models.CommandContext;
using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Services;

namespace NeuralJourney.Logic.Commands.Players
{
    public class PlayerCommandStrategy : IPlayerCommandStrategy
    {
        private readonly ICommandFactory _commandFactory;
        private readonly IMessageService _messageService;
        private readonly IOpenAIService _openAIService;

        public PlayerCommandStrategy(ICommandFactory commandFactory, IMessageService messageService, IOpenAIService openAIService)
        {
            _commandFactory = commandFactory;
            _messageService = messageService;
            _openAIService = openAIService;
        }

        public async Task ExecuteAsync(string playerInput, Player player)
        {
            var responseMessage = string.Empty;

            try
            {
                var completionText = await _openAIService.GetCommandCompletionTextAsync(playerInput);

                var commandContext = GetCommandContextFromCompletionText(completionText, player);

                var command = _commandFactory.CreateCommand(commandContext);

                var result = await command.ExecuteAsync();

                responseMessage = $"{commandContext.ExecutionMessage}\n{result.AdditionalMessage ?? string.Empty}".Trim();
            }
            catch (InvalidCommandException ex)
            {
                Console.WriteLine(ex.Message);
                responseMessage = ex.Message;
            }
            catch (MissingParameterException ex)
            {
                Console.WriteLine(ex.Message);
                responseMessage = ex.Message;
            }
            catch (InvalidParameterException ex)
            {
                Console.WriteLine(ex.Message);
                responseMessage = ex.Message;
            }
            catch (CommandMappingException ex)
            {
                Console.WriteLine(ex.Message);
                responseMessage = ex.Message;
            }
            catch (PlayerActionException ex)
            {
                Console.WriteLine(ex.Message);
                responseMessage = ex.Message;
            }
            finally
            {
                await _messageService.SendMessageAsync(player.GetStream(), responseMessage);
            }
        }

        private static CommandContext GetCommandContextFromCompletionText(string completionText, Player player)
        {
            var mainParts = completionText.Split('|');

            if (mainParts.Length <= 1)
                throw new InvalidCompletionDataException(completionText, "Could not split text into Command and SuccessMessage");

            var commandParts = mainParts[0].Split(new[] { "(", ")" }, StringSplitOptions.None) ?? Array.Empty<string>();

            var commandIdentifierText = commandParts[0].TrimStart();
            var commandParams = Array.Empty<string>();

            if (!Enum.TryParse(commandIdentifierText, true, out CommandIdentifierEnum commandIdentifier))
                throw new InvalidCompletionDataException(completionText, string.Format("Could not parse the command \'{0}\' to {1}", commandIdentifier, nameof(CommandIdentifierEnum)));

            if (commandParts.Length >= 2)
                commandParams = commandParts[1].Split(',');

            if (commandParams.Any(string.IsNullOrEmpty))
                throw new InvalidCompletionDataException(completionText, "Has empty command parameters");

            var successMessage = mainParts[1];

            if (string.IsNullOrEmpty(successMessage))
                throw new InvalidCompletionDataException(completionText, "SuccessMessage is blank");

            return new CommandContext(commandIdentifier, commandParams, successMessage);
        }
    }
}