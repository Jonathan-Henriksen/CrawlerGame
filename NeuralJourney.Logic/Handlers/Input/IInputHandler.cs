using NeuralJourney.Library.Models.Commands;
using NeuralJourney.Library.Models.World;

namespace NeuralJourney.Logic.Handlers.Input
{
    public interface IInputHandler
    {
        public event Action<CommandContext>? OnInputReceived;

        public Task HandleInputAsync(Player? player = default, CancellationToken cancellationToken = default);
    }
}