namespace NeuralJourney.Logic.Engines.Interfaces
{
    public interface IEngine : IDisposable
    {
        public Task Run(CancellationToken cancellationToken);

        public Task Stop();
    }
}