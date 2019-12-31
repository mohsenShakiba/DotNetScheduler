using System;

namespace CronScheduler
{
    public class JobSpecification: IJobSpecification
    {
        public Type JobType { get; }
        public string CronExpression { get; }

        public JobSpecification(Type jobType, string cronExpression)
        {
            JobType = jobType;
            CronExpression = cronExpression;
        }
        
    }
}