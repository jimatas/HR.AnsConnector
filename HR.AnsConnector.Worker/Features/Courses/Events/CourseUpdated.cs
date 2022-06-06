using HR.AnsConnector.Features.Common.Commands;
using HR.AnsConnector.Infrastructure;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Events;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Courses.Events
{
    public class CourseUpdated : IEvent
    {
        public CourseUpdated(CourseRecord course, ApiResponse<Course> apiResponse)
        {
            Course = course;
            ApiResponse = apiResponse;
        }

        public CourseRecord Course { get; }
        public ApiResponse<Course> ApiResponse { get; }
    }

    public class CourseUpdatedHandler : IEventHandler<CourseUpdated>
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public CourseUpdatedHandler(ICommandDispatcher commandDispatcher, ILogger<CourseUpdatedHandler> logger)
        {
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(CourseUpdated e, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Handling {nameof(CourseUpdated)} event by dispatching {nameof(MarkAsHandled)} command.");

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
