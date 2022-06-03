using HR.AnsConnector.Features.Courses.Events;
using HR.AnsConnector.Infrastructure;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Events;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Courses.Commands
{
    public class CreateCourse : ICommand
    {
        public CreateCourse(CourseRecord course)
        {
            Course = course;
        }

        public CourseRecord Course { get; }
    }

    public class CreateCourseHandler : ICommandHandler<CreateCourse>
    {
        private readonly IApiClient apiClient;
        private readonly IEventDispatcher eventDispatcher;
        private readonly ILogger logger;

        public CreateCourseHandler(IApiClient apiClient, IEventDispatcher eventDispatcher, ILogger<CreateCourseHandler> logger)
        {
            this.apiClient = apiClient;
            this.eventDispatcher = eventDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(CreateCourse command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Creating {Course} in Ans.", command.Course);

            var apiResponse = await apiClient.CreateCourseAsync(command.Course, cancellationToken).WithoutCapturingContext();
            if (apiResponse.IsSuccessStatusCode())
            {
                logger.LogInformation("{Course} was successfully created in Ans.", command.Course);
            }
            else if (apiResponse.IsErrorStatusCode())
            {
                logger.LogWarning("Received {StatusMessage} while attempting to create {Course} in Ans. [{ValidationErrors}]",
                    apiResponse.GetStatusMessage(),
                    command.Course,
                    apiResponse.GetValidationErrorsAsSingleMessage());
            }

            await eventDispatcher.DispatchAsync(new CourseCreated(command.Course, apiResponse), cancellationToken).WithoutCapturingContext();
        }
    }
}
