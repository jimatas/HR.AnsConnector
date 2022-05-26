using HR.AnsConnector.Features.Departments;
using HR.AnsConnector.Features.Users;
using HR.Common.Utilities;

using Microsoft.EntityFrameworkCore;

namespace HR.AnsConnector.Infrastructure.Persistence
{
    public class Database : IDatabase
    {
        private readonly AnsDbContext dbContext;
        private readonly IHostEnvironment environment;
        private readonly ILogger logger;

        public Database(AnsDbContext dbContext, IHostEnvironment environment, ILogger<Database> logger)
        {
            this.dbContext = dbContext;
            this.environment = environment;
            this.logger = logger;
        }

        public async Task<UserRecord?> GetNextUserAsync(CancellationToken cancellationToken = default)
        {
            var sprocName = GetEnvironmentSpecificSprocName("sync_out_ans_{0}_user_GetNextEvents");
            logger.LogDebug("Executing stored procedure '{SprocName}'.", sprocName);

            var users = await dbContext.Users.FromSqlRaw(sprocName).AsNoTracking().ToListAsync(cancellationToken).WithoutCapturingContext();
            return users.SingleOrDefault();
        }

        public async Task<DepartmentRecord?> GetNextDepartmentAsync(CancellationToken cancellationToken = default)
        {
            var sprocName = GetEnvironmentSpecificSprocName("sync_out_ans_{0}_department_GetNextEvents");
            logger.LogDebug("Executing stored procedure '{SprocName}'.", sprocName);

            var departments = await dbContext.Departments.FromSqlRaw(sprocName).AsNoTracking().ToListAsync(cancellationToken).WithoutCapturingContext();
            return departments.SingleOrDefault();
        }

        public async Task MarkAsHandledAsync(
            bool success,
            string? message,
            int? id,
            int? eventId,
            CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Executing stored procedure 'sync_event_MarkHandled'.");

            await dbContext.Database.ExecuteSqlInterpolatedAsync($"sync_event_MarkHandled {eventId}, {success}, {id?.ToString()}, {message}", cancellationToken).WithoutCapturingContext();
        }

        private string GetEnvironmentSpecificSprocName(string sprocName)
        {
            return string.Format(sprocName, environment.IsProduction() ? "prod" : "test");
        }
    }
}
