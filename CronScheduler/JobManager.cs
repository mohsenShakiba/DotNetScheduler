using System;
using System.Threading;
using System.Threading.Tasks;
using CronScheduler.Cron;
using Microsoft.Extensions.Logging;
using Scheduler.Base.Config;

namespace CronScheduler
{
    public class JobManager: IDisposable, IEquatable<JobManager>
    {

        public IJob Job { get; }
        public string CronExpression { get; }
        public JobConfiguration JobConfiguration { get; }
        
        private readonly CrontabSchedule _crontabSchedule;
        private readonly Timer _timer;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly ILogger _logger;
        private bool _isJobBeingProcessed;
        private bool _isDisposed;

        public JobManager(IJob job, string cronExpression, JobConfiguration jobConfiguration, ILogger logger)
        {
            _logger = logger;
            Job = job;
            CronExpression = cronExpression;
            JobConfiguration = jobConfiguration;
            _crontabSchedule = CrontabSchedule.Parse(CronExpression);
            _timer = new Timer(RunNextAsync);
        }
        
        public void SetupTimer()
        {
            if (_isDisposed)
                return;
            var nextOccurrence = _crontabSchedule.GetNextOccurrence(DateTime.Now);
            var timeSpanTillNexOccurence = nextOccurrence - DateTime.Now;
            _timer.Change(timeSpanTillNexOccurence, TimeSpan.Zero);
        }

        private async void RunNextAsync(object timeSpanUntilNextOccurence)
        {
            var taskDuration = (TimeSpan) timeSpanUntilNextOccurence;
            SetupTimer();
            if (!_isJobBeingProcessed || JobConfiguration.AllowParallelExecution)
                await RunJobAsync(taskDuration);
        }

        private async Task RunJobAsync(TimeSpan timeSpan)
        {
            _cancellationTokenSource = new CancellationTokenSource(timeSpan);
            _isJobBeingProcessed = true;
            try
            {
                await Job.RunAsync(_cancellationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning($"{this} took too long");
            }
            catch (Exception e)
            {
                _logger.LogError($"{this} throw an exception\n{e}");
            }
            finally
            {
                _isJobBeingProcessed = false;
            }
        }

        public void Dispose()
        {
            _isDisposed = true;
            _timer?.Dispose();
            _cancellationTokenSource?.Cancel();
        }

        public override string ToString()
        {
            return $"Job: {nameof(Job)}, with cron expression: {CronExpression}";
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