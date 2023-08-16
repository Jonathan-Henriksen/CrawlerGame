﻿using NeuralJourney.Core.Exceptions;
using NeuralJourney.Core.Interfaces.Handlers;
using NeuralJourney.Core.Interfaces.Services;
using NeuralJourney.Core.Models.World;
using Serilog;

namespace NeuralJourney.Infrastructure.Handlers
{
    public class PlayerInputHandler : IInputHandler<Player>
    {
        private readonly IMessageService _messageService;
        private readonly ILogger _logger;

        public event Action<string, Player>? OnInputReceived;
        public event Action<Player>? OnClosedConnection;

        public PlayerInputHandler(IMessageService messageService, ILogger logger)
        {
            _messageService = messageService;
            _logger = logger.ForContext<PlayerInputHandler>();
        }

        public async Task HandleInputAsync(Player player, CancellationToken cancellationToken)
        {
            var client = player.GetClient();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var input = await _messageService.ReadMessageAsync(client, cancellationToken);

                    if (_messageService.IsCloseConnectionMessage(input))
                    {
                        OnClosedConnection?.Invoke(player);
                        return;
                    }

                    if (string.IsNullOrEmpty(input))
                        continue;

                    OnInputReceived?.Invoke(input, player);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (MessageException ex)
                {
                    await _messageService.SendMessageAsync(client, ex.Message, cancellationToken);
                    await Task.Delay(500, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Unexpected error while handling player input");

                    await _messageService.SendCloseConnectionAsync(client, cancellationToken);

                    return;
                }
            }
        }
    }
}