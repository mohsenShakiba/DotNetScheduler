using System;
using System.Threading;
using System.Threading.Tasks;
using CronScheduler.Cron;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Scheduler.Base.Config;

namespace CronScheduler
{
    public class JobManager: IDisposable, IEquatable<JobManager>
    {

        public Type JobType { get; }
        public string CronExpression { get; }
        public JobConfiguration JobConfiguration { get; }
        
        private readonly CrontabSchedule _crontabSchedule;
        private readonly Timer _timer;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private bool _isJobBeingProcessed;
        private bool _isDisposed;
        private DateTime _nextOccurenceExedutionDate;

        public JobManager(Type jobType, string cronExpression, JobConfiguration jobConfiguration, IServiceProvider serviceProvider)
        {
            JobType = jobType;
            _serviceProvider = serviceProvider;
            _logger = serviceProvider.GetRequiredService<ILogger<JobManager>>();
            CronExpression = cronExpression;
            JobConfiguration = jobConfiguration;
            _crontabSchedule = CrontabSchedule.Parse(CronExpression);
            _timer = new Timer(RunNextAsync);
        }
        
        public void SetupTimer()
        {
            if (_isDisposed)
                return;
            var now = DateTime.Now;
            _nextOccurenceExedutionDate = _crontabSchedule.GetNextOccurrence(now);
            var timeSpanTillNexOccurence = _nextOccurenceExedutionDate - now;
            if (timeSpanTillNexOccurence < TimeSpan.MinValue)
                timeSpanTillNexOccurence = TimeSpan.FromSeconds(1);
            _timer.Change(timeSpanTillNexOccurence, TimeSpan.Zero);
            _logger.LogInformation($"Job: {JobShortName} scheduled for {timeSpanTillNexOccurence.TotalSeconds} seconds from now");
        }

        private async void RunNextAsync(object timer)
        {
            var nextOccurrence = _crontabSchedule.GetNextOccurrence(_nextOccurenceExedutionDate);
            var jobDeadline = nextOccurrence - _nextOccurenceExedutionDate;
            _logger.LogInformation($"Job: {JobShortName} is now running with {jobDeadline.TotalSeconds} seconds till next execution");
            SetupTimer();
            if (!_isJobBeingProcessed || JobConfiguration.AllowParallelExecution)
                await RunJobAsync(jobDeadline);
        }

        private async Task RunJobAsync(TimeSpan timeSpan)
        {
            _cancellationTokenSource = new CancellationTokenSource(timeSpan);
            _isJobBeingProcessed = true;
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var job = (IJob)scope.ServiceProvider.GetService(JobType);
                await job.RunAsync(_cancellationTokenSource.Token);
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
                _cancellationTokenSource.Dispose();
                _isJobBeingProcessed = false;
            }
        }

        public void Dispose()
        {
            _isDisposed = true;
            _timer?.Dispose();
            _cancellationTokenSource?.Cancel();
        }

        public string JobShortName => $"{JobType}";

        public override string ToString()
        {
            return $"Job: {JobShortName}, with cron expression: {CronExpression}";
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((JobType.GetHashCode()) * 397) ^ CronExpression.GetHashCode();
            }
        }

        public bool Equals(JobManager other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return JobType.FullName == other.JobType.FullName && CronExpression == other.CronExpression;
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