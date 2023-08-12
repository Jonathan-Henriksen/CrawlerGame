﻿using NeuralJourney.Core.Constants.Messages;
using NeuralJourney.Core.Enums.Commands;
using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Interfaces.Handlers;
using NeuralJourney.Core.Interfaces.Services;
using NeuralJourney.Core.Models.Commands;
using NeuralJourney.Core.Models.World;
using Serilog;
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
            _logger = logger;

            // Subscribe to input events
            _inputHandler.OnInputReceived += DispatchCommand;
            _inputHandler.OnClosedConnection += RemovePlayer;
        }

        public void AddPlayer(TcpClient playerClient, CancellationToken cancellationToken)
        {
            var player = new Player(playerClient);

            _players.Add(player);

            // Start background task to dispatch commands on input
            _ = _inputHandler.HandleInputAsync(player, cancellationToken);
        }

        private void DispatchCommand(string input, Player player)
        {
            var context = new CommandContext(input, CommandTypeEnum.Player, player);
            _commandDispatcher.DispatchCommand(context);

            _logger.Debug(DebugMessageTemplates.PlayerDispatchedCommand, player.Name, input);
        }

        private void RemovePlayer(Player player)
        {
            var stream = player.GetStream();

            _logger.Information(InfoMessageTemplates.ClientDisconnected, stream.Socket.RemoteEndPoint);

            stream.Socket.Close();
            stream.Socket.Dispose();

            _players.Remove(player);
        }

        public void Dispose()
        {
            // Ubsubscribe from events
            _inputHandler.OnInputReceived -= DispatchCommand;
            _inputHandler.OnClosedConnection -= RemovePlayer;

            foreach (var player in _players)
            {
                _messageService.SendCloseConnectionAsync(player.GetStream());
            }

            _logger.Debug(DebugMessageTemplates.DispoedOfType, GetType().Name);
        }
    }
}