using NeuralJourney.Core.Constants;
using NeuralJourney.Core.Extensions;
using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Interfaces.Handlers;
using NeuralJourney.Core.Interfaces.Services;
using NeuralJourney.Core.Models.Commands;
using NeuralJourney.Core.Models.LogProperties;
using NeuralJourney.Core.Models.World;
using Serilog;
using Serilog.Context;
using System.Collections.Concurrent;
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

        private readonly ConcurrentDictionary<Guid, Player> _players = new();

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

            var addressLogger = LogContext.PushProperty("IpAddress", playerClient.GetRemoteIp());
            try
            {
                player = await InitPlayerAsync(playerClient, cancellationToken);

                _players.TryAdd(player.Id, player);
            }
            catch (OperationCanceledException) // Cancelled when player sends 'Close Connection' during initialization
            {
                _logger.Warning(ServerLogMessages.Warning.PlayerLeftEarly);

                playerClient.Close();
                return;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ServerLogMessages.Error.PlayerAddFailed);

                playerClient.Close();
                return;
            }
            finally
            {
                addressLogger.Dispose();
            }

            var playerContext = new PlayerContext(player.Name, player.Id, playerClient.GetRemoteIp());

            // Start background task to notify about new input
            using (LogContext.PushProperty(nameof(PlayerContext), playerContext, true))
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

            using (LogContext.PushProperty(nameof(CommandContext), context, true))
                _commandDispatcher.DispatchCommandAsync(context);
        }

        private void RemovePlayer(Player player)
        {
            var client = player.Client;

            _logger.Information(ServerLogMessages.Info.PlayerRemoved, player.Name);

            client.Close();
            client.Dispose();

            _players.Remove(player.Id, out var _);
        }

        private async Task<Player> InitPlayerAsync(TcpClient client, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = await _messageService.ReadMessageAsync(client, cancellationToken);

                // Check if connection was closed instead
                if (_messageService.IsCloseConnectionMessage(message))
                    throw new OperationCanceledException();

                if (!_messageService.IsHandshake(message, out var name, out var id) || string.IsNullOrEmpty(name) || !id.HasValue)
                    continue;

                // Validate that name is not in use
                if (_players.Any(p => p.Value.Name == name))
                {
                    await _messageService.SendMessageAsync(client, PlayerMessages.WelcomeFlow.NameAlreadyTaken, cancellationToken);
                    continue;
                }

                // Validate the name format
                if (string.IsNullOrEmpty(name) || !PlayerNameValidation().IsMatch(name))
                {
                    await _messageService.SendMessageAsync(client, PlayerMessages.WelcomeFlow.InvalidNameFormat, cancellationToken);
                    continue;
                }

                // Notify player that name and id have been verified
                var player = new Player(client, name, id.Value);
                var playerContext = new PlayerContext(player.Name, player.Id, client.GetRemoteIp());

                using (LogContext.PushProperty(nameof(PlayerContext), playerContext, true))
                {
                    await _messageService.SendHandshake(client, player.Name, player.Id, cancellationToken);

                    await _messageService.SendMessageAsync(client, string.Format(PlayerMessages.WelcomeFlow.WelcomeNameMessage, player.Name), cancellationToken);
                };

                return player;
            }

            return new Player(client, $"Player({client.GetRemoteIp()})", default);
        }

        public async Task RemoveAllPlayers()
        {
            foreach (var player in _players.Values)
            {
                var client = player.Client;

                using (LogContext.PushProperty(nameof(PlayerContext), new PlayerContext(player.Name, player.Id, client.GetRemoteIp())))
                {
                    await _messageService.SendCloseConnectionAsync(client);

                    RemovePlayer(player);
                }
            }
        }

        public void Dispose()
        {
            // Ubsubscribe from events
            _inputHandler.OnInputReceived -= DispatchCommand;
            _inputHandler.OnClosedConnection -= RemovePlayer;
            // Gracefully disconnect players
            foreach (var player in _players.Values)
            {
                _messageService.SendCloseConnectionAsync(player.Client);
            }

            _players.Clear();

            _logger.Debug(SystemMessages.DispoedOfType, GetType().Name);
        }

        [GeneratedRegex("^[A-Za-z0-9_\\-]{3,16}$")]
        private static partial Regex PlayerNameValidation();
    }
}