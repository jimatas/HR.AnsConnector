using HR.Common.Utilities;

namespace HR.AnsConnector.Infrastructure
{
    public class BatchSettings
    {
        private int batchSize = 10;

        /// <summary>
        /// Proposed batch size per resource (users, departments, etc.) and action performed (both creates and updates together, or deletes).
        /// Default value is 10. 
        /// Minimum value is 1.
        /// </summary>
        public int BatchSize
        {
            get => batchSize;
            set => batchSize = Ensure.Argument.NotOutOfRange(() => value, lowerBound: 1);
        }

        private TimeSpan timeDelayBetweenRuns = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Default value is 1 second.
        /// Minimum value is 00:00:00, or no delay.
        /// </summary>
        public TimeSpan TimeDelayBetweenRuns
        {
            get => timeDelayBetweenRuns;
            set => timeDelayBetweenRuns = Ensure.Argument.NotOutOfRange(() => value, lowerBound: TimeSpan.Zero);
        }
    }
}
