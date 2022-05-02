namespace HR.AnsConnector.Infrastructure
{
    public class BatchSettings
    {
        public static readonly TimeSpan DefaultTimeDelayBetweenRuns = TimeSpan.FromSeconds(1);
        public const int DefaultBatchSize = 10;

        /// <summary>
        /// Proposed batch size per resource (users, departments, etc.) and action performed (either creates and updates together, or deletes).
        /// </summary>
        public int BatchSize { get; set; } = DefaultBatchSize;
        public TimeSpan TimeDelayBetweenRuns { get; set; } = DefaultTimeDelayBetweenRuns;
    }
}
