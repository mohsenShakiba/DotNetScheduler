using System;

namespace CronScheduler.Cron
{
    [Serializable]
    public enum CrontabFieldKind
    {
        Second,
        Minute,
        Hour,
        Day,
        Month,
        DayOfWeek
    }
}