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
            using var scopedService = _serviceProvider.CreateScope();
            var job = scopedService.ServiceProvider.GetService(jobType) as IJob;
            var jobLogger = scopedService.ServiceProvider.GetService<ILogger<IJob>>();
            var jobManager = new JobManager(job, cronExpression, jobConfiguration ?? new JobConfiguration(), jobLogger);
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