namespace NeuralJourney.Core.Interfaces.Services
{
    public interface IClockService
    {
        public void Start();

        public void Stop();

        public void Reset();

        public TimeOnly GetTime();
    }
}