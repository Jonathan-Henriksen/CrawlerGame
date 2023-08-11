namespace NeuralJourney.Core.Interfaces.Handlers
{
    public interface IInputHandler<T>
    {
        public event Action<string, T>? OnInputReceived;
        public event Action<T>? OnClosedConnection;

        public Task HandleInputAsync(T client, CancellationToken cancellationToken = default);
    }
}