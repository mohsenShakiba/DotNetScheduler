using System;
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Base;

namespace Scheduler.Jobs
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