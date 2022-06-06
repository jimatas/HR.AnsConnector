using HR.Common.Cqrs;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Common.Commands
{
    public class MarkAsHandledWithRetry : ICommandHandlerWrapper<MarkAsHandled>
    {
        private const int RetryCount = 4;
        private const int RetryDelayInSeconds = 15;

        private readonly ILogger<MarkAsHandledWithRetry> logger;

        public MarkAsHandledWithRetry(ILogger<MarkAsHandledWithRetry> logger)
        {
            this.logger = logger;
        }

        public async Task HandleAsync(MarkAsHandled command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            await HandleWithRetryAsync(command, next, tryCount: RetryCount + 1, cancellationToken).WithoutCapturingContext();
        }

        private async Task HandleWithRetryAsync(MarkAsHandled command, HandlerDelegate next, int tryCount, CancellationToken cancellationToken)
        {
            try
            {
                await next().WithoutCapturingContext();
            }
            catch (Exception ex) when (IsTimeoutException(ex) && tryCount > 0)
            {
                logger.LogWarning("Timeout expired waiting for {CommandName} command to finish. " +
                    "Attempting retry in {RetryDelay} secs.", nameof(MarkAsHandled), RetryDelayInSeconds);

                await Task.Delay(TimeSpan.FromSeconds(RetryDelayInSeconds), cancellationToken).WithoutCapturingContext();
                await HandleWithRetryAsync(command, next, tryCount - 1, cancellationToken).WithoutCapturingContext();
            }
        }

        private static bool IsTimeoutException(Exception ex)
        {
            return ex.Message.Contains("Timeout Expired", StringComparison.OrdinalIgnoreCase)
                || (ex.InnerException is not null && IsTimeoutException(ex.InnerException));
        }
    }
}
