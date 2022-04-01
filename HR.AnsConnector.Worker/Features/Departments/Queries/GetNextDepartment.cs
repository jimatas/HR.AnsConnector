using HR.AnsConnector.Infrastructure.Persistence;
using HR.Common.Cqrs.Queries;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Departments.Queries
{
    public class GetNextDepartment : IQuery<DepartmentRecord?>
    {
    }

    public class GetNextDepartmentHandler : IQueryHandler<GetNextDepartment, DepartmentRecord?>
    {
        private readonly IDatabase database;
        private readonly ILogger logger;

        public GetNextDepartmentHandler(IDatabase database, ILogger<GetNextDepartmentHandler> logger)
        {
            this.database = database;
            this.logger = logger;
        }

        public async Task<DepartmentRecord?> HandleAsync(GetNextDepartment query, CancellationToken cancellationToken)
        {
            var department = await database.GetNextDepartmentAsync(cancellationToken).WithoutCapturingContext();
            if (department is not null)
            {
                logger.LogInformation("Retrieved {Department} from database.", department);
            }

            return department;
        }
    }
}
