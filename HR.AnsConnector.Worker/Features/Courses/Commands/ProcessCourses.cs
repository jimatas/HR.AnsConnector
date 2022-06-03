using HR.AnsConnector.Features.Courses.Queries;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Queries;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Courses.Commands
{
    public class ProcessCourses : ICommand
    {
        public ProcessCourses(int batchSize, bool isDeleteContext = false)
        {
            BatchSize = batchSize;
            IsDeleteContext = isDeleteContext;
        }

        public int BatchSize { get; set; }
        public bool IsDeleteContext { get; }
    }

    public class ProcessCoursesHandler : ICommandHandler<ProcessCourses>
    {
        private readonly IQueryDispatcher queryDispatcher;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public ProcessCoursesHandler(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, ILogger<ProcessCoursesHandler> logger)
        {
            this.queryDispatcher = queryDispatcher;
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(ProcessCourses command, CancellationToken cancellationToken)
        {
            int created = 0,
                updated = 0,
                deleted = 0;

            logger.LogInformation("Processing courses to {Action}.", command.IsDeleteContext ? "delete" : "create or update");

            for (var i = 0; i < command.BatchSize; i++)
            {
                var nextCourse = await queryDispatcher.DispatchAsync(new GetNextCourse(), cancellationToken).WithoutCapturingContext();
                if (nextCourse is null
                    || ((nextCourse.IsToBeCreated() || nextCourse.IsToBeUpdated()) && command.IsDeleteContext)
                    || nextCourse.IsToBeDeleted() && !command.IsDeleteContext)
                {
                    logger.LogInformation("No more courses to {Action}. Ending batch run.", command.IsDeleteContext ? "delete" : "create or update");
                    break;
                }

                if (nextCourse.IsToBeCreated())
                {
                    await commandDispatcher.DispatchAsync(new CreateCourse(nextCourse), cancellationToken).WithoutCapturingContext();
                    created++;
                }
                else if (nextCourse.IsToBeUpdated())
                {
                    await commandDispatcher.DispatchAsync(new UpdateCourse(nextCourse), cancellationToken).WithoutCapturingContext();
                    updated++;
                }
                else if (nextCourse.IsToBeDeleted())
                {
                    await commandDispatcher.DispatchAsync(new DeleteCourse(nextCourse), cancellationToken).WithoutCapturingContext();
                    deleted++;
                }
            }

            logger.LogInformation("Processed {Processed} course(s) in total.", created + updated + deleted);
            if (command.IsDeleteContext)
            {
                logger.LogDebug("Deleted {Deleted} course(s).", deleted);
            }
            else
            {
                logger.LogDebug("Created {Created} course(s).", created);
                logger.LogDebug("Updated {Updated} course(s).", updated);
            }
        }
    }
}
