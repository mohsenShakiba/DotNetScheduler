using System;
using System.Threading.Tasks;
using CronScheduler.HostedServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scheduler.Base;

namespace CronScheduler.Extensions
{
    public static class SchedulerExtension
    {
        public static IServiceCollection AddScheduler(this IServiceCollection serviceCollection, Action<JobSchedulerBuilder> jobScheduleBuilder)
        {
            serviceCollection.AddHostedService<JobScheduleHostedService>();
            return serviceCollection.AddSingleton<IJobScheduler, JobScheduler>(sp =>
            {
                var scheduler = new JobScheduler(sp);
                var builder = new JobSchedulerBuilder(serviceCollection, scheduler);
                jobScheduleBuilder(builder);
                return scheduler;
            });
        }

    }
}