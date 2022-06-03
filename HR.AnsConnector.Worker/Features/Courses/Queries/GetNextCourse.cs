using HR.AnsConnector.Infrastructure.Persistence;
using HR.Common.Cqrs.Queries;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Courses.Queries
{
    public class GetNextCourse : IQuery<CourseRecord?>
    {
    }

    public class GetNextCourseHandler : IQueryHandler<GetNextCourse, CourseRecord?>
    {
        private readonly IDatabase database;
        private readonly ILogger logger;

        public GetNextCourseHandler(IDatabase database, ILogger<GetNextCourseHandler> logger)
        {
            this.database = database;
            this.logger = logger;
        }

        public async Task<CourseRecord?> HandleAsync(GetNextCourse query, CancellationToken cancellationToken)
        {
            var course = await database.GetNextCourseAsync(cancellationToken).WithoutCapturingContext();
            if (course is not null)
            {
                logger.LogInformation("Retrieved {Course} from database.", course);
            }

            return course;
        }
    }
}
