using NeuralJourney.Library.Constants;
using NeuralJourney.Library.Enums.Commands;
using NeuralJourney.Library.Exceptions.Commands;
using NeuralJourney.Library.Models.Commands;
using NeuralJourney.Library.Models.World;
using NeuralJourney.Logic.Services;
using Serilog;
using System.Text.RegularExpressions;

namespace NeuralJourney.Logic.Commands.Players
{
    public class PlayerCommandStrategy : IPlayerCommandStrategy
    {
        private readonly ICommandFactory _commandFactory;
        private readonly IMessageService _messageService;
        private readonly IOpenAIService _openAIService;
        private readonly ILogger _logger;

        public PlayerCommandStrategy(ICommandFactory commandFactory, IMessageService messageService, IOpenAIService openAIService, ILogger logger)
        {
            _commandFactory = commandFactory;
            _messageService = messageService;
            _openAIService = openAIService;
            _logger = logger;
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

                _logger.Information(InfoMessageTemplates.ExecutedCommand, commandContext.CommandType, commandContext.CommandIdentifier);

                responseMessage = $"{commandContext.ExecutionMessage}\n{result.AdditionalMessage ?? string.Empty}".Trim();

            }
            catch (InvalidCompletionTextException ex)
            {
                _logger.Error(ex, ex.Message);
                responseMessage = ex.Message;
            }
            catch (InvalidCommandException ex)
            {
                _logger.Error(ex, ex.Message);
                responseMessage = ex.Message;
            }
            catch (MissingParameterException ex)
            {
                _logger.Error(ex, ex.Message);
                responseMessage = ex.Message;
            }
            catch (InvalidParameterException ex)
            {
                _logger.Error(ex, ex.Message);
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
                throw new InvalidCompletionTextException(completionText, "Invalid command format.");

            var commandIdentifierText = match.Groups["commandIdentifier"].Value;
            if (!Enum.TryParse(commandIdentifierText, true, out CommandIdentifierEnum commandIdentifier))
                throw new InvalidCompletionTextException(completionText, $"Could not parse the command '{commandIdentifierText}' to {nameof(CommandIdentifierEnum)}");

            var commandParams = match.Groups["params"].Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            var successMessage = match.Groups["successMessage"].Value;

            return new CommandContext(commandIdentifier, commandParams, successMessage);
        }

    }
}