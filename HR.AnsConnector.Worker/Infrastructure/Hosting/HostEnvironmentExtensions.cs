namespace HR.AnsConnector.Infrastructure.Hosting
{
    internal static class HostEnvironmentExtensions
    {
        /// <summary>
        /// Gets the appropriate value for that part of a stored procedure's name that is used to indicate its intended hosting environment.
        /// For example, "prod" in "sync_out_ans_prod_user_GetNextEvents".
        /// </summary>
        /// <example>test, prod</example>
        /// <param name="environment"></param>
        /// <returns></returns>
        public static string GetStoredProcedureEnvironmentName(this IHostEnvironment environment)
        {
            return environment.IsProduction() ? "prod" : "test";
        }
    }
}
