﻿using System;
 using Scheduler.Base.Config;

namespace CronScheduler
{
    public interface IJobScheduler: IDisposable
    {
        void RegisterJob(Type jobType, string cronExpression, JobConfiguration jobConfiguration = null);
    }
}