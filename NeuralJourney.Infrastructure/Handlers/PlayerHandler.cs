using NeuralJourney.Core.Constants;
using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Interfaces.Handlers;
using NeuralJourney.Core.Interfaces.Services;
using NeuralJourney.Core.Models.Commands;
using NeuralJourney.Core.Models.World;
using Serilog;
using Serilog.Context;
using System.Net.Sockets;

namespace NeuralJourney.Infrastructure.Handlers
{
    public class PlayerHandler : IPlayerHandler
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

        public void AddPlayer(TcpClient playerClient, CancellationToken cancellationToken)
        {
            var player = new Player(playerClient);

            _players.Add(player);

            using (LogContext.PushProperty("Player", player, true))
            {
                _logger.Information(ServerLogTemplates.Info.PlayerAdded, player.Name);

                // Start background task to notify about new input
                _ = _inputHandler.HandleInputAsync(player, cancellationToken).ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        _logger.Error(t.Exception?.InnerException, ServerLogTemplates.Error.PlayerInputFailed);

                        RemovePlayer(player);
                    }
                }, cancellationToken);
            }
        }

        private void DispatchCommand(string input, Player player)
        {
            var context = new CommandContext(input, player);

            _logger.Debug(ServerLogTemplates.Debug.DispatchedPlayerCommand, player.Name);

            _commandDispatcher.DispatchCommand(context);
        }

        private void RemovePlayer(Player player)
        {
            var client = player.GetClient();

            _logger.Information(ServerLogTemplates.Info.PlayerRemoved, client.Client.RemoteEndPoint);

            client.Close();
            client.Dispose();

            _players.Remove(player);
        }

        public async Task StopAsync()
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

            _logger.Debug(SystemMessageTemplates.DispoedOfType, GetType().Name);
        }
    }
}