namespace NeuralJourney.Core.Interfaces.Engines
{
    public interface IEngine : IDisposable
    {
        public Task Run(CancellationToken cancellationToken);

        public Task StopAsync();
    }
}