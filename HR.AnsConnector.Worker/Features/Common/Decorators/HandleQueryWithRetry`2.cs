using HR.Common.Cqrs;
using HR.Common.Cqrs.Queries;
using HR.Common.Utilities;

using System.Data.Common;

namespace HR.AnsConnector.Features.Common.Decorators
{
    public class HandleQueryWithRetry<TQuery, TResult> : IQueryHandlerWrapper<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private const int RetryCount = 4; // Excluding the original try.
        private const int RetryDelayInSeconds = 15;

        private readonly ILogger logger;

        public HandleQueryWithRetry(ILogger<HandleQueryWithRetry<TQuery, TResult>> logger)
        {
            this.logger = logger;
        }

        public async Task<TResult> HandleAsync(TQuery query, HandlerDelegate<TResult> next, CancellationToken cancellationToken)
        {
            return await HandleWithRetryAsync(query, next, RetryCount, cancellationToken).WithoutCapturingContext();
        }

        private async Task<TResult> HandleWithRetryAsync(TQuery query, HandlerDelegate<TResult> next, int retries, CancellationToken cancellationToken)
        {
            try
            {
                return await next().WithoutCapturingContext();
            }
            catch (Exception ex) when (IsTimeoutException(ex) && retries > 0)
            {
                logger.LogWarning("Timeout expired waiting for {QueryName} to finish. "
                    + "Attempting retry in {RetryDelay} secs.", query.GetType().Name, RetryDelayInSeconds);

                await Task.Delay(TimeSpan.FromSeconds(RetryDelayInSeconds), cancellationToken).WithoutCapturingContext();
                return await HandleWithRetryAsync(query, next, retries - 1, cancellationToken).WithoutCapturingContext();
            }
        }

        private static bool IsTimeoutException(Exception ex)
        {
            return (ex is DbException && ex.Message.Contains("Timeout Expired", StringComparison.OrdinalIgnoreCase))
                || (ex.InnerException is not null && IsTimeoutException(ex.InnerException));
        }
    }
}
