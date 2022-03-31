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

        public Task MarkAsHandledAsync(
            bool success, 
            string statusMessage, 
            string? errorMessage, 
            int? id, 
            int? eventId, 
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
