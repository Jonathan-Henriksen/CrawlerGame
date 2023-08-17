using NeuralJourney.Core.Constants;
using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Interfaces.Handlers;
using NeuralJourney.Core.Interfaces.Services;
using NeuralJourney.Core.Models.Commands;
using NeuralJourney.Core.Models.World;
using Serilog;
using Serilog.Context;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace NeuralJourney.Infrastructure.Handlers
{
    public partial class PlayerHandler : IPlayerHandler
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IInputHandler<Player> _inputHandler;
        private readonly IMessageService _messageService;
        private readonly ILogger _logger;

        private readonly List<Player> _players = new();

        public PlayerHandler(ICommandDispatcher commandDispatcher, IInputHandler<Player> inputHandler, IMessageService messageService, ILogger logger)
        {
            _commandDispatcher = commandDispatcher;
            _inputHandler = inputHandler;
            _messageService = messageService;
            _logger = logger.ForContext<PlayerHandler>();

            // Subscribe to input events
            _inputHandler.OnInputReceived += DispatchCommand;
            _inputHandler.OnClosedConnection += RemovePlayer;
        }

        public async void AddPlayer(TcpClient playerClient, CancellationToken cancellationToken)
        {
            Player? player = null;

            var addressLogger = _logger.ForContext("Address", playerClient.Client.RemoteEndPoint);

            try
            {
                var playerName = await RequestPlayerNameAsync(playerClient, cancellationToken);

                player = new Player(playerClient, playerName);

                _players.Add(player);
            }
            catch (OperationCanceledException) // Cancelled when player sends 'Close Connection' during name request
            {
                addressLogger.Warning(ServerLogMessages.Warning.PlayerLeftEarly);

                playerClient.Close();
                return;
            }
            catch (Exception ex)
            {
                addressLogger.Error(ex, ServerLogMessages.Error.PlayerAddFailed);

                playerClient.Close();
                return;
            }

            // Start background task to notify about new input
            using (LogContext.PushProperty("Player", player, true))
            {
                _logger.Information(ServerLogMessages.Info.PlayerAdded, player.Name);

                _ = _inputHandler.HandleInputAsync(player, cancellationToken).ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        _logger.Error(t.Exception?.InnerException, ServerLogMessages.Error.PlayerInputFailed);
                        RemovePlayer(player);
                    }
                }, cancellationToken);
            }
        }


        private void DispatchCommand(string input, Player player)
        {
            var context = new CommandContext(input, player);

            _logger.Debug(ServerLogMessages.Debug.DispatchedPlayerCommand, player.Name);

            _commandDispatcher.DispatchCommand(context);
        }

        private void RemovePlayer(Player player)
        {
            var client = player.GetClient();

            _logger.Information(ServerLogMessages.Info.PlayerRemoved, player.Name);

            client.Close();
            client.Dispose();

            _players.Remove(player);
        }

        private async Task<string> RequestPlayerNameAsync(TcpClient client, CancellationToken cancellationToken)
        {
            await _messageService.SendMessageAsync(client, PlayerMessages.WelcomeFlow.WelcomeMessage, cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                var playerName = await _messageService.ReadMessageAsync(client, cancellationToken);

                if (_messageService.IsCloseConnectionMessage(playerName))
                    throw new OperationCanceledException();

                if (!_players.Any(p => p.Name == playerName) && !string.IsNullOrEmpty(playerName) && PlayerNameValidation().IsMatch(playerName))
                {
                    await _messageService.SendMessageAsync(client, string.Format(PlayerMessages.WelcomeFlow.WelcomeNameMessage, playerName));

                    return playerName;
                }

                if (_players.Any(p => p.Name == playerName))
                {
                    await _messageService.SendMessageAsync(client, PlayerMessages.WelcomeFlow.NameAlreadyTaken, cancellationToken);
                }
                else
                {
                    await _messageService.SendMessageAsync(client, PlayerMessages.WelcomeFlow.InvalidNameFormat, cancellationToken);
                }
            }

            return $"Player({client.Client.RemoteEndPoint})";
        }

        public async Task RemoveAllPlayers()
        {
            foreach (var player in _players)
            {
                await _messageService.SendCloseConnectionAsync(player.GetClient());
                RemovePlayer(player);
            }
        }

        public void Dispose()
        {
            // Ubsubscribe from events
            _inputHandler.OnInputReceived -= DispatchCommand;
            _inputHandler.OnClosedConnection -= RemovePlayer;

            // Gracefully disconnect players
            foreach (var player in _players)
            {
                _messageService.SendCloseConnectionAsync(player.GetClient());
            }

            _players.Clear();

            _logger.Debug(SystemMessages.DispoedOfType, GetType().Name);
        }

        [GeneratedRegex("^[A-Za-z0-9_\\-]{3,16}$")]
        private static partial Regex PlayerNameValidation();
    }
}