using HR.AnsConnector.Infrastructure;
using HR.Common.Cqrs;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

using Microsoft.Extensions.Options;

using System.Data.Common;

namespace HR.AnsConnector.Features.Common.Commands
{
    public class MarkAsHandledWithRetry : ICommandHandlerWrapper<MarkAsHandled>
    {
        private readonly RecoverySettings recoverySettings;
        private readonly ILogger logger;

        public MarkAsHandledWithRetry(IOptionsSnapshot<RecoverySettings> recoverySettings, ILogger<MarkAsHandledWithRetry> logger)
        {
            this.recoverySettings = recoverySettings.Get(RecoverySettings.Names.CommandTimeoutExpired);
            this.logger = logger;
        }

        public async Task HandleAsync(MarkAsHandled command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            await HandleWithRetryAsync(command, next, recoverySettings.RetryAttempts, cancellationToken).WithoutCapturingContext();
        }

        private async Task HandleWithRetryAsync(MarkAsHandled command, HandlerDelegate next, int retries, CancellationToken cancellationToken)
        {
            try
            {
                await next().WithoutCapturingContext();
            }
            catch (Exception ex) when (IsTimeoutException(ex) && retries > 0)
            {
                logger.LogWarning("Timeout expired waiting for MarkAsHandled command to finish. "
                    + "Attempting retry in {RetryDelay} secs.", recoverySettings.RetryDelay.TotalSeconds);

                await Task.Delay(recoverySettings.RetryDelay, cancellationToken).WithoutCapturingContext();
                await HandleWithRetryAsync(command, next, retries - 1, cancellationToken).WithoutCapturingContext();
            }
        }

        private static bool IsTimeoutException(Exception ex)
        {
            return (ex is DbException && ex.Message.Contains("Timeout Expired", StringComparison.OrdinalIgnoreCase))
                || (ex.InnerException is not null && IsTimeoutException(ex.InnerException));
        }
    }
}
