using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CronScheduler.HostedServices
{
    public class JobScheduleHostedService: IHostedService
    {
        private readonly IServiceProvider _serviceScopeFactory;
        private readonly IJobScheduler _jobScheduler;

        public JobScheduleHostedService(IServiceProvider serviceProvider)
        {
            _serviceScopeFactory = serviceProvider;
            _jobScheduler = new JobScheduler(serviceProvider);
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var jobSpecifications = _serviceScopeFactory.GetServices<IJobSpecification>();
            
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