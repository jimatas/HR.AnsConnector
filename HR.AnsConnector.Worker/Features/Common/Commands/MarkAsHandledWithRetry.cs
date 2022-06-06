using HR.Common.Cqrs;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Common.Commands
{
    public class MarkAsHandledWithRetry : ICommandHandlerWrapper<MarkAsHandled>
    {
        private readonly ILogger<MarkAsHandledWithRetry> logger;

        public MarkAsHandledWithRetry(ILogger<MarkAsHandledWithRetry> logger)
        {
            this.logger = logger;
        }

        public async Task HandleAsync(MarkAsHandled command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            await HandleWithRetryAsync(command, next, retries: 5).WithoutCapturingContext();
        }

        private async Task HandleWithRetryAsync(MarkAsHandled command, HandlerDelegate next, int retries)
        {
            try
            {
                await next().WithoutCapturingContext();
            }
            catch (Exception ex) when (IsTimeoutException(ex) && retries > 0)
            {
                logger.LogWarning($"Timeout expired waiting for {nameof(MarkAsHandled)} call. Attempting retry.");
                await HandleWithRetryAsync(command, next, retries - 1).WithoutCapturingContext();
            }
        }

        private static bool IsTimeoutException(Exception ex)
        {
            return ex.Message.Contains("Timeout Expired", StringComparison.OrdinalIgnoreCase)
                || (ex.InnerException is not null && IsTimeoutException(ex.InnerException));
        }
    }
}
