using Scheduler.Base.Config;

namespace Scheduler.Base
{
    public interface IJobScheduler
    {
        void RegisterJob<T>(string cronExpression, JobConfiguration jobConfiguration = null) where T : IJob;
    }
}