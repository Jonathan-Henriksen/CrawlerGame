using CrawlerGame.Logic.Options;
using CrawlerGame.Logic.Services.Interfaces;
using System.Timers;
using Timer = System.Timers.Timer;

namespace CrawlerGame.Logic.Services
{
    public class ClockService : IClockService
    {
        private readonly Timer _timer;

        private TimeOnly Time;

        public ClockService(GameOptions gameOptions)
        {
            Time = new TimeOnly(12, 0);
            _timer = new Timer(gameOptions.SecondsPerMinute * 1000);
            _timer.Elapsed += OnTimerElapsed;
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public void Reset()
        {
            _timer.Stop();
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