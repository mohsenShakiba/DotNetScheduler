using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Scheduler.Base;

namespace CronScheduler.Extensions
{
    public static class SchedulerExtension
    {
        public static IServiceCollection AddScheduler(this IServiceCollection serviceCollection, Action<IJobScheduler> jobScheduleBuilder)
        {
            return serviceCollection.AddSingleton<IJobScheduler, JobScheduler>(l =>
            {
                var scopedFactory = l.GetService<IServiceScopeFactory>();
                var jobScheduler = new JobScheduler(scopedFactory);
                jobScheduleBuilder(jobScheduler);
                return jobScheduler;
            });
        }
        
    }
}