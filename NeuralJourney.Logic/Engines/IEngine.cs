namespace NeuralJourney.Logic.Engines
{
    public interface IEngine
    {
        public Task<IEngine> Init(CancellationToken cancellationToken = default);

        public Task Run(CancellationToken cancellationToken);

        public Task Stop();
    }
}