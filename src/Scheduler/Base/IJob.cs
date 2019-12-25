using System;
using System.Threading;
using System.Threading.Tasks;

namespace Scheduler.Base
{
    public interface IJob
    {
        Task RunAsync(CancellationToken token);
    }
}