namespace NeuralJourney.Logic.Handlers
{
    public interface IInputHandler<T>
    {
        public event Action<string, T>? OnInputReceived;

        public Task HandleInputAsync(T client, CancellationToken cancellationToken = default);
    }
}