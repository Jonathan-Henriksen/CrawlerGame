using NeuralJourney.Library.Enums.Commands;
using NeuralJourney.Library.Exceptions.Commands;
using NeuralJourney.Library.Exceptions.PlayerActions;
using NeuralJourney.Library.Models.Commands;
using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Services;
using System.Text.RegularExpressions;

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
            var regexPattern = @"^(?<commandIdentifier>\w+)\((?<params>[^\)]+)\)\|(?<successMessage>.+?)$";
            var match = Regex.Match(completionText, regexPattern);

            if (!match.Success)
                throw new InvalidCompletionDataException(completionText, "Invalid command format.");

            var commandIdentifierText = match.Groups["commandIdentifier"].Value;
            if (!Enum.TryParse(commandIdentifierText, true, out CommandIdentifierEnum commandIdentifier))
                throw new InvalidCompletionDataException(completionText, $"Could not parse the command '{commandIdentifierText}' to {nameof(CommandIdentifierEnum)}");

            var commandParams = match.Groups["params"].Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            var successMessage = match.Groups["successMessage"].Value;

            return new CommandContext(commandIdentifier, commandParams, successMessage);
        }

    }
}