﻿using Scheduler.Base.Config;

namespace CronScheduler
{
    public interface IJobScheduler
    {
        void RegisterJob<T>(string cronExpression, JobConfiguration jobConfiguration = null) where T : IJob;
    }
}