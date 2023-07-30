using CrawlerGame.Logic.Services.Interfaces;
using System.Timers;
using Timer = System.Timers.Timer;

namespace CrawlerGame.Logic.Services
{
    public class ClockService : IClockService
    {
        private TimeOnly inGameTime;
        private bool isRunning;
        private readonly Timer timer;

        public ClockService()
        {
            inGameTime = new TimeOnly(0, 0);
            isRunning = false;
            timer = new Timer(15000);
            timer.Elapsed += OnTimerElapsed;
        }

        public void Start()
        {
            if (!isRunning)
            {
                isRunning = true;
                timer.Start();
            }
        }

        public void Pause()
        {
            isRunning = false;
            timer.Stop();
        }

        public void Resume()
        {
            if (!isRunning)
            {
                isRunning = true;
                timer.Start();
            }
        }

        public TimeOnly GetInGameTime()
        {
            return inGameTime;
        }

        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            if (!isRunning)
                return;

            inGameTime = inGameTime.AddMinutes(1);

        }
    }
}
