using Microsoft.Extensions.DependencyInjection;

namespace CronScheduler
{
    public class JobSchedulerBuilder
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly IJobScheduler _jobScheduler;

        public JobSchedulerBuilder(IServiceCollection serviceCollection, IJobScheduler jobScheduler)
        {
            _serviceCollection = serviceCollection;
            _jobScheduler = jobScheduler;
        }

        public void AddJob<T>(string cronExpression) where T: class, IJob
        {
            _serviceCollection.AddScoped<IJob, T>();
            _jobScheduler.RegisterJob<T>(cronExpression);
        }
    }
}