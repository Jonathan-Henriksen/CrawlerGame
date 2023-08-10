using NeuralJourney.Logic.Handlers.Interfaces;
using NeuralJourney.Logic.Services.Interfaces;
using System.Net.Sockets;

namespace NeuralJourney.Logic.Handlers
{
    public class NetworkInputHandler : IInputHandler<NetworkStream>
    {
        private readonly IMessageService _messageService;

        public event Action<string, NetworkStream>? OnInputReceived;

        public NetworkInputHandler(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task HandleInputAsync(NetworkStream stream, CancellationToken cancellationToken = default)
        {
            if (!stream.CanRead)
                throw new InvalidOperationException("Failed to handle stream input. Reason: Cannot read from stream");

            while (!cancellationToken.IsCancellationRequested)
            {
                var input = await _messageService.ReadMessageAsync(stream, cancellationToken);
                if (_messageService.IsCloseConnectionMessage(input))
                {
                    return;
                }

                if (string.IsNullOrEmpty(input))
                    continue;

                OnInputReceived?.Invoke(input, stream);
            }
        }
    }
}