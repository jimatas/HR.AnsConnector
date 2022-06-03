using HR.AnsConnector.Features.Common.Commands;
using HR.AnsConnector.Infrastructure;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Events;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Studies.Events
{
    public class StudyDeleted : IEvent
    {
        public StudyDeleted(StudyRecord study, ApiResponse<Study> apiResponse)
        {
            Study = study;
            ApiResponse = apiResponse;
        }

        public StudyRecord Study { get; }
        public ApiResponse<Study> ApiResponse { get; }
    }

    public class StudyDeletedHandler : IEventHandler<StudyDeleted>
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public StudyDeletedHandler(ICommandDispatcher commandDispatcher, ILogger<StudyDeletedHandler> logger)
        {
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(StudyDeleted e, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Handling {nameof(StudyDeleted)} event by dispatching {nameof(MarkAsHandled)} command.");

            await commandDispatcher.DispatchAsync(
                new MarkAsHandled(
                    e.ApiResponse.IsSuccessStatusCode(),
                    e.ApiResponse.GetStatusMessage(),
                    e.ApiResponse.GetValidationErrorsAsSingleMessage(),
                    e.ApiResponse.Data?.Id,
                    e.Study.EventId),
                cancellationToken).WithoutCapturingContext();
        }
    }
}
