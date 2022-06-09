using System.ComponentModel.DataAnnotations;

namespace HR.AnsConnector.Infrastructure
{
    public class BatchSettings
    {
        /// <summary>
        /// Proposed batch size per resource (users, departments, etc.) and action performed (both creates and updates together, or deletes).
        /// Default value is 10.
        /// Minimum value is 1.
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "The field {0} must be greater than or equal to {1}.")]
        public int BatchSize { get; set; } = 10;

        /// <summary>
        /// Default value is 1 second.
        /// Valid range of values is between 00:00:00 (no delay) and 1.00:00:00 (1 day).
        /// </summary>
        [Range(typeof(TimeSpan), "00:00:00", "1.00:00:00")]
        public TimeSpan TimeDelayBetweenRuns { get; set; } = TimeSpan.FromSeconds(1);
    }
}
