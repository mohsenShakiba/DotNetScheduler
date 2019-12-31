using System;

namespace CronScheduler
{
    public interface IJobSpecification
    {
        Type JobType { get; }
        string CronExpression { get; }
    }
}