using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Scheduler.Base.Extensions
{
    public static class SchedulerExtension
    {
        public static IServiceCollection AddScheduler(this IServiceCollection services, EventHandler<UnobservedTaskExceptionEventArgs> unobservedTaskExceptionHandler)
        {
            return services.AddSingleton<IJobScheduler, JobScheduler>();
        }
    }
}