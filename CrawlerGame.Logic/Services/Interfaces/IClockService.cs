﻿namespace CrawlerGame.Logic.Services.Interfaces
{
    public interface IClockService
    {
        public void Start();

        public void Stop();

        public void Reset();

        public TimeOnly GetTime();
    }
}