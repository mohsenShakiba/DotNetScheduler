using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Scheduler.Base.Config;

namespace CronScheduler
{
    public class JobScheduler : IJobScheduler
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly IList<JobManager> _jobsManagerList = new List<JobManager>();

        public JobScheduler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void RegisterJob(Type jobType, string cronExpression, JobConfiguration jobConfiguration = null)
        {
            var jobManager = new JobManager(jobType, cronExpression, jobConfiguration ?? new JobConfiguration(), _serviceProvider);
            _jobsManagerList.Add(jobManager);
            jobManager.SetupTimer();
        }


        public void Dispose()
        {
            foreach (var jobManager in _jobsManagerList)
            {
                jobManager.Dispose();
            }
        }
    }
}