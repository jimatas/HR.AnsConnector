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
            }
        }
    }
}
