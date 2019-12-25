using System;
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Base.Cron;

namespace Scheduler.Base
{
    public class JobManager: IDisposable, IEquatable<JobManager>
    {

        public IJob Job { get; }
        public string CronExpression { get; }

        private bool _isRunning = false;
        private Timer _timer;
        private CancellationTokenSource _cancellationTokenSource;
        
        public JobManager(IJob job, string cronExpression)
        {
            Job = job;
            CronExpression = cronExpression;
        }

        public void SetupNextScheduling()
        {
            var timeSpanTillNexOccurence = GetNextOccurence();
            _timer?.Dispose();
            Console.WriteLine($"next job will run in {timeSpanTillNexOccurence}");
            _timer = new Timer(RunNextAsync, timeSpanTillNexOccurence, timeSpanTillNexOccurence, 
                TimeSpan.Zero);
        }

        private async void RunNextAsync(object o)
        {
            var taskDuration = (TimeSpan) o;
            await RunAsync(taskDuration);
            SetupNextScheduling();
        }

        private async Task RunAsync(TimeSpan timeSpan)
        {
            var cts = new CancellationTokenSource(timeSpan);
            try
            {
                await Job.RunAsync(cts.Token);
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine($"{this} took too long to run");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public TimeSpan GetNextOccurence()
        {
            var scheduler = CrontabSchedule.Parse(CronExpression);
            var nextOccurrence = scheduler.GetNextOccurrence(DateTime.Now);
            return nextOccurrence - DateTime.Now;
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _cancellationTokenSource?.Cancel();
        }

        public override string ToString()
        {
            return $"Job: {nameof(Job)}, runs every: {CronExpression}";
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Job != null ? Job.GetHashCode() : 0) * 397) ^ CronExpression.GetHashCode();
            }
        }

        public bool Equals(JobManager other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Job.GetType().FullName == other.Job.GetType().FullName && CronExpression == other.CronExpression;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((JobManager) obj);
        }
    }
}