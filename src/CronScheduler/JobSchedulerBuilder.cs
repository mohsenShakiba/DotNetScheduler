﻿using Microsoft.Extensions.DependencyInjection;

namespace CronScheduler
{
    public class JobSchedulerBuilder
    {
        private readonly IServiceCollection _serviceCollection;

        public JobSchedulerBuilder(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public void AddJob<T>(string cronExpression) where T: class, IJob
        {
            _serviceCollection.AddTransient<T>();
            _serviceCollection.AddScoped<IJobSpecification>(_ => new JobSpecification(typeof(T), cronExpression));
        }
    }
}