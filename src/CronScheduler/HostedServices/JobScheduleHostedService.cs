using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CronScheduler.HostedServices
{
    public class JobScheduleHostedService: IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IJobScheduler _jobScheduler;

        public JobScheduleHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _jobScheduler = new JobScheduler(serviceProvider);
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            using var scoped = _serviceProvider.CreateScope();
            var jobSpecifications = scoped.ServiceProvider.GetServices<IJobSpecification>();
            
            foreach(var jobSpecification in jobSpecifications)
                _jobScheduler.RegisterJob(jobSpecification.JobType, jobSpecification.CronExpression);
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _jobScheduler.Dispose();
            return Task.CompletedTask;
        }
    }
}