using HR.AnsConnector.Features.Common.Commands;
using HR.AnsConnector.Infrastructure;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Events;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Courses.Events
{
    public class CourseCreated : IEvent
    {
        public CourseCreated(CourseRecord course, ApiResponse<Course> apiResponse)
        {
            Course = course;
            ApiResponse = apiResponse;
        }

        public CourseRecord Course { get; }
        public ApiResponse<Course> ApiResponse { get; }
    }

    public class CourseCreatedHandler : IEventHandler<CourseCreated>
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public CourseCreatedHandler(ICommandDispatcher commandDispatcher, ILogger<CourseCreatedHandler> logger)
        {
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(CourseCreated e, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Handling {nameof(CourseCreated)} event by dispatching {nameof(MarkAsHandled)} command.");

            await commandDispatcher.DispatchAsync(
                new MarkAsHandled(
                    e.ApiResponse.IsSuccessStatusCode(),
                    e.ApiResponse.GetStatusMessage(),
                    e.ApiResponse.GetValidationErrorsAsSingleMessage(),
                    e.ApiResponse.Data?.Id,
                    e.Course.EventId),
                cancellationToken).WithoutCapturingContext();
        }
    }
}
