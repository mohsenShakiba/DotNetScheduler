using System;
using System.Collections.Generic;

namespace Scheduler.Base
{
    public class JobScheduler
    {

        private IList<JobManager> _jobsManager = new List<JobManager>();
        
        public void RegisterJob<T>(string cronExpression) where T: IJob, new()
        {
            var jobManager = new JobManager(new T(), cronExpression);
            _jobsManager.Add(jobManager);
        }

        public void UseScheduler()
        {
            foreach (var jobManager in _jobsManager)
            {
                jobManager.SetupNextScheduling();
            }
        }
    }
}