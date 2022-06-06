using HR.AnsConnector.Features.Common.Commands;
using HR.AnsConnector.Infrastructure;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Events;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Courses.Events
{
    public class CourseDeleted : IEvent
    {
        public CourseDeleted(CourseRecord course, ApiResponse<Course> apiResponse)
        {
            Course = course;
            ApiResponse = apiResponse;
        }

        public CourseRecord Course { get; }
        public ApiResponse<Course> ApiResponse { get; }
    }

    public class CourseDeletedHandler : IEventHandler<CourseDeleted>
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public CourseDeletedHandler(ICommandDispatcher commandDispatcher, ILogger<CourseDeletedHandler> logger)
        {
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(CourseDeleted e, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Handling {nameof(CourseDeleted)} event by dispatching {nameof(MarkAsHandled)} command.");

            await commandDispatcher.DispatchAsync(
                new MarkAsHandled(
                    (int)e.Course.EventId!,
                    e.ApiResponse.IsSuccessStatusCode(),
                    e.ApiResponse.Data?.Id,
                    e.ApiResponse.ValidationErrors.Any() ? e.ApiResponse.GetValidationErrorsAsSingleMessage() : e.ApiResponse.GetStatusMessage()),
                cancellationToken).WithoutCapturingContext();
        }
    }
}
