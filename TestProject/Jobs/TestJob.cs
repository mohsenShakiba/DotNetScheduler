using System;
using System.Threading;
using System.Threading.Tasks;
using CronScheduler;

namespace TestProject.Jobs
{
    public class TestJob: IJob
    {
        public async Task RunAsync(CancellationToken token)
        {
            await Task.Delay(3000, token);

            Console.WriteLine($"test was run at {DateTime.Now}");
        }
    }
}