﻿namespace HR.AnsConnector.Infrastructure
{
    public class RecoverySettings
    {
        public static class Names
        {
            public const string CommandTimeoutExpired = nameof(CommandTimeoutExpired);
            public const string TransientHttpFault = nameof(TransientHttpFault);
        }

        /// <summary>
        /// The number of attempts to retry the failed/faulted action.
        /// Default is 4 attempts.
        /// </summary>
        public int RetryAttempts { get; set; } = 4;

        /// <summary>
        /// The time delay before a retry attempt.
        /// Default value is 15 seconds.
        /// </summary>
        public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(15);
    }
}