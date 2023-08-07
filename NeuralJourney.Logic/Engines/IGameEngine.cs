namespace NeuralJourney.Logic.Engines
{
    public interface IGameEngine
    {
        public Task Run();

        public void Stop();
    }
}