using NeuralJourney.Core.Interfaces.Services;
using NeuralJourney.Core.Models.Options;
using System.Timers;
using Timer = System.Timers.Timer;

namespace NeuralJourney.Infrastructure.Services
{
    public class ClockService : IClockService
    {
        private readonly Timer Timer;

        private TimeOnly Time;

        public ClockService(GameOptions gameOptions)
        {
            Time = new TimeOnly(12, 0);
            Timer = new Timer(gameOptions.SecondsPerMinute * 1000);
            Timer.Elapsed += OnTimerElapsed;
        }

        public void Start()
        {
            Timer.Start();
        }

        public void Stop()
        {
            Timer.Stop();
        }

        public void Reset()
        {
            Timer.Stop();
            Time = new TimeOnly(12, 0);
        }

        public TimeOnly GetTime()
        {
            return Time;
        }

        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            Time = Time.AddMinutes(1);
        }
    }
}