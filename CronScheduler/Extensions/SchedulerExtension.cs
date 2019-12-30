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
            return serviceCollection.AddSingleton<IJobScheduler, JobScheduler>(sp =>
            {
                var scheduler = new JobScheduler(sp);
                jobScheduleBuilder(scheduler);
                return scheduler;
            });
        }
        
    }
}