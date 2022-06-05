using HR.AnsConnector.Features.Courses;
using HR.AnsConnector.Features.Departments;
using HR.AnsConnector.Features.Studies;
using HR.AnsConnector.Features.Users;
using HR.AnsConnector.Infrastructure.Hosting;
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
            var sprocName = string.Format("sync_out_ans_{0}_user_GetNextEvents", environment.GetStoredProcedureEnvironmentName());
            logger.LogDebug("Executing stored procedure '{SprocName}'.", sprocName);

            var users = await dbContext.Users.FromSqlRaw(sprocName).AsNoTracking().ToListAsync(cancellationToken).WithoutCapturingContext();
            return users.SingleOrDefault();
        }

        public async Task<DepartmentRecord?> GetNextDepartmentAsync(CancellationToken cancellationToken = default)
        {
            var sprocName = string.Format("sync_out_ans_{0}_department_GetNextEvents", environment.GetStoredProcedureEnvironmentName());
            logger.LogDebug("Executing stored procedure '{SprocName}'.", sprocName);

            var departments = await dbContext.Departments.FromSqlRaw(sprocName).AsNoTracking().ToListAsync(cancellationToken).WithoutCapturingContext();
            return departments.SingleOrDefault();
        }

        public async Task<StudyRecord?> GetNextStudyAsync(CancellationToken cancellationToken = default)
        {
            var sprocName = string.Format("sync_out_ans_{0}_studies_GetNextEvents", environment.GetStoredProcedureEnvironmentName());
            logger.LogDebug("Executing stored procedure '{SprocName}'.", sprocName);

            var studies = await dbContext.Studies.FromSqlRaw(sprocName).AsNoTracking().ToListAsync(cancellationToken).WithoutCapturingContext();
            return studies.SingleOrDefault();
        }

        public async Task<CourseRecord?> GetNextCourseAsync(CancellationToken cancellationToken = default)
        {
            var sprocName = string.Format("sync_out_ans_{0}_course_GetNextEvents", environment.GetStoredProcedureEnvironmentName());
            logger.LogDebug("Executing stored procedure '{SprocName}'.", sprocName);

            var courses = await dbContext.Courses.FromSqlRaw(sprocName).AsNoTracking().ToListAsync(cancellationToken).WithoutCapturingContext();
            return courses.SingleOrDefault();
        }

        public async Task MarkAsHandledAsync(
            int eventId,
            bool success,
            int? id,
            string? message,
            CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Executing stored procedure 'sync_event_MarkHandled'.");

            await dbContext.Database.ExecuteSqlInterpolatedAsync($"sync_event_MarkHandled {eventId}, {success}, {id?.ToString()}, {message}", cancellationToken).WithoutCapturingContext();
        }
    }
}
