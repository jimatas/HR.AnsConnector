using System.Data.Common;

namespace HR.AnsConnector.Infrastructure
{
    internal static class ExceptionExtensions
    {
        public static bool IsTimeoutException(this Exception ex)
        {
            return (ex is DbException && ex.Message.Contains("Timeout Expired", StringComparison.OrdinalIgnoreCase))
                || (ex.InnerException is not null && IsTimeoutException(ex.InnerException));
        }
    }
}
