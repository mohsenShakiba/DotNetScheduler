using System.Threading;
using System.Threading.Tasks;

namespace CronScheduler
{
    public interface IJob
    {
        Task RunAsync(CancellationToken token);
    }
}