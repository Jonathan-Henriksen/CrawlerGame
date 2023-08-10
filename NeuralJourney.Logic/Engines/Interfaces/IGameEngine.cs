namespace NeuralJourney.Logic.Engines.Interfaces
{
    public interface IGameEngine
    {
        public Task Run();

        public void Stop();
    }
}