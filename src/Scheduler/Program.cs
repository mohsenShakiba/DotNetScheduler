using System;
using Scheduler.Base;
using Scheduler.Jobs;

namespace Scheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            var scheduler = new JobScheduler();
            scheduler.RegisterJob<TestJob>("*/10 * * * * *");
            
            scheduler.UseScheduler();

            Console.ReadLine();
        }
    }
}