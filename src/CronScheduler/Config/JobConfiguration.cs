namespace Scheduler.Base.Config
{
    public class JobConfiguration
    {
        /// <summary>
        /// this flag indicated whether the scheduler should run a new instance of the job
        /// if due time has been reached and the last job is still in execution
        /// if true the scheduler will allow multiple instances of the job to run simeltaniously
        /// if false the scheduler will wait for the last job to end before executing the next one
        /// default: false
        /// </summary>
        public bool AllowParallelExecution { get; }
        public bool DebugMode { get; }

        public JobConfiguration()
        {
            AllowParallelExecution = false;
            DebugMode = false;
        }

        public JobConfiguration(bool throwOnTimeout, bool debugMode)
        {
            AllowParallelExecution = throwOnTimeout;
            DebugMode = debugMode;
        }
    }
}