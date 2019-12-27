using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Scheduler.Base.Config;

namespace Scheduler.Base
{
    public class JobScheduler : IJobScheduler
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        private readonly IList<JobManager> _jobsManager = new List<JobManager>();

        public JobScheduler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void RegisterJob<T>(string cronExpression, JobConfiguration jobConfiguration = null) where T : IJob
        {
            using var scopedService = _serviceScopeFactory.CreateScope();
            var job = scopedService.ServiceProvider.GetService<T>();
            var jobLogger = scopedService.ServiceProvider.GetService<ILogger<T>>();
            var jobManager = new JobManager(job, cronExpression, jobConfiguration ?? new JobConfiguration(), jobLogger);
            _jobsManager.Add(jobManager);
        }

        public void UseScheduler()
        {
            foreach (var jobManager in _jobsManager)
            {
                jobManager.SetupTimer();
            }
        }

    }
}