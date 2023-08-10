namespace NeuralJourney.Logic.Engines.Interfaces
{
    public interface IEngine
    {
        public Task<IEngine> Init(CancellationToken cancellationToken = default);

        public Task Run(CancellationToken cancellationToken);

        public Task Stop();
    }
}