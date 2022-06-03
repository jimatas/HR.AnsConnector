using HR.AnsConnector.Features.Courses.Events;
using HR.AnsConnector.Infrastructure;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Events;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Courses.Commands
{
    public class DeleteCourse : ICommand
    {
        public DeleteCourse(CourseRecord course)
        {
            Course = course;
        }

        public CourseRecord Course { get; }
    }

    public class DeleteCourseHandler : ICommandHandler<DeleteCourse>
    {
        private readonly IApiClient apiClient;
        private readonly IEventDispatcher eventDispatcher;
        private readonly ILogger logger;

        public DeleteCourseHandler(IApiClient apiClient, IEventDispatcher eventDispatcher, ILogger<DeleteCourseHandler> logger)
        {
            this.apiClient = apiClient;
            this.eventDispatcher = eventDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(DeleteCourse command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Deleting {Course} from Ans.", command.Course);

            var apiResponse = await apiClient.DeleteCourseAsync(command.Course, cancellationToken).WithoutCapturingContext();
            if (apiResponse.IsSuccessStatusCode())
            {
                logger.LogInformation("{Course} was successfully deleted from Ans.", command.Course);
            }
            else if (apiResponse.IsErrorStatusCode())
            {
                logger.LogWarning("Received {StatusMessage} while attempting to delete {Course} from Ans. [{ValidationErrors}]",
                    apiResponse.GetStatusMessage(),
                    command.Course,
                    apiResponse.GetValidationErrorsAsSingleMessage());
            }

            await eventDispatcher.DispatchAsync(new CourseDeleted(command.Course, apiResponse), cancellationToken).WithoutCapturingContext();
        }
    }
}
