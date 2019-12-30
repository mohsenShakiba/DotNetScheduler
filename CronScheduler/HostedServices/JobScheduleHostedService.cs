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

        public JobScheduleHostedService(IServiceProvider serviceProvider)
        {
            _serviceScopeFactory = serviceProvider;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var scheduledService = _serviceScopeFactory.GetService<IJobScheduler>();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}