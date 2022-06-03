using HR.AnsConnector.Features.Courses.Events;
using HR.AnsConnector.Infrastructure;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Events;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Courses.Commands
{
    public class UpdateCourse : ICommand
    {
        public UpdateCourse(CourseRecord course)
        {
            Course = course;
        }

        public CourseRecord Course { get; }
    }

    public class UpdateCourseHandler : ICommandHandler<UpdateCourse>
    {
        private readonly IApiClient apiClient;
        private readonly IEventDispatcher eventDispatcher;
        private readonly ILogger logger;

        public UpdateCourseHandler(IApiClient apiClient, IEventDispatcher eventDispatcher, ILogger<UpdateCourseHandler> logger)
        {
            this.apiClient = apiClient;
            this.eventDispatcher = eventDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(UpdateCourse command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Updating {Course} in Ans.", command.Course);

            var apiResponse = await apiClient.UpdateCourseAsync(command.Course, cancellationToken).WithoutCapturingContext();
            if (apiResponse.IsSuccessStatusCode())
            {
                logger.LogInformation("{Course} was successfully updated in Ans.", command.Course);
            }
            else if (apiResponse.IsErrorStatusCode())
            {
                logger.LogWarning("Received {StatusMessage} while attempting to update {Course} in Ans. [{ValidationErrors}]",
                    apiResponse.GetStatusMessage(),
                    command.Course,
                    apiResponse.GetValidationErrorsAsSingleMessage());
            }

            await eventDispatcher.DispatchAsync(new CourseUpdated(command.Course, apiResponse), cancellationToken).WithoutCapturingContext();
        }
    }
}
