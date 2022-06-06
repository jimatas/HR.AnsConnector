using HR.Common.Cqrs;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Common.Commands
{
    public class MarkAsHandledWithRetry : ICommandHandlerWrapper<MarkAsHandled>
    {
        private const int RetryCount = 4; // Excluding the original try.
        private const int RetryDelayInSeconds = 15;

        private readonly ILogger<MarkAsHandledWithRetry> logger;

        public MarkAsHandledWithRetry(ILogger<MarkAsHandledWithRetry> logger)
        {
            this.logger = logger;
        }

        public async Task HandleAsync(MarkAsHandled command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            await HandleWithRetryAsync(command, next, RetryCount, cancellationToken).WithoutCapturingContext();
        }

        private async Task HandleWithRetryAsync(MarkAsHandled command, HandlerDelegate next, int retries, CancellationToken cancellationToken)
        {
            try
            {
                await next().WithoutCapturingContext();
            }
            catch (Exception ex) when (IsTimeoutException(ex) && retries > 0)
            {
                logger.LogWarning("Timeout expired waiting for {CommandName} command to finish. " +
                    "Attempting retry in {RetryDelay} secs.", nameof(MarkAsHandled), RetryDelayInSeconds);

                await Task.Delay(TimeSpan.FromSeconds(RetryDelayInSeconds), cancellationToken).WithoutCapturingContext();
                await HandleWithRetryAsync(command, next, retries - 1, cancellationToken).WithoutCapturingContext();
            }
        }

        private static bool IsTimeoutException(Exception ex)
        {
            return ex.Message.Contains("Timeout Expired", StringComparison.OrdinalIgnoreCase)
                || (ex.InnerException is not null && IsTimeoutException(ex.InnerException));
        }
    }
}
