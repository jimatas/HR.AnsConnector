using HR.AnsConnector.Features.Departments;
using HR.AnsConnector.Features.Users;
using HR.Common.Utilities;

using Microsoft.EntityFrameworkCore;

namespace HR.AnsConnector.Infrastructure.Persistence
{
    public class Database : IDatabase
    {
        private readonly AnsDbContext dbContext;

        public Database(AnsDbContext dbContext) => this.dbContext = dbContext;

        public async Task<UserRecord?> GetNextUserAsync(CancellationToken cancellationToken = default)
        {
            var users = await dbContext.Users.FromSqlRaw("sync_ans_user_GetNextEvents").AsNoTracking().ToListAsync(cancellationToken).WithoutCapturingContext();
            return users.SingleOrDefault();
        }

        public Task<DepartmentRecord?> GetNextDepartmentAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task MarkAsHandledAsync(
            bool success,
            string? message,
            int? id,
            int? eventId,
            CancellationToken cancellationToken = default)
        {
            await dbContext.Database.ExecuteSqlInterpolatedAsync($"sync_event_MarkHandled {eventId}, {success}, {id?.ToString()}, {message}", cancellationToken).WithoutCapturingContext();
        }
    }
}
