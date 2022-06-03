using HR.AnsConnector.Features.Common.Commands;
using HR.AnsConnector.Infrastructure;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Events;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Studies.Events
{
    public class StudyUpdated : IEvent
    {
        public StudyUpdated(StudyRecord study, ApiResponse<Study> apiResponse)
        {
            Study = study;
            ApiResponse = apiResponse;
        }

        public StudyRecord Study { get; }
        public ApiResponse<Study> ApiResponse { get; }
    }

    public class StudyUpdatedHandler : IEventHandler<StudyUpdated>
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public StudyUpdatedHandler(ICommandDispatcher commandDispatcher, ILogger<StudyUpdatedHandler> logger)
        {
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(StudyUpdated e, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Handling {nameof(StudyUpdated)} event by dispatching {nameof(MarkAsHandled)} command.");

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
