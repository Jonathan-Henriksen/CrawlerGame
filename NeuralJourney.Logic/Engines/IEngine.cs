namespace NeuralJourney.Logic.Engines
{
    public interface IEngine
    {
        public Task<IEngine> Init(CancellationTokenSource cancellationTokenSource);

        public Task Run();

        public Task Stop();
    }
}