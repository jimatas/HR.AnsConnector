using HR.AnsConnector.Features.Common.Commands;
using HR.AnsConnector.Infrastructure;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Events;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Studies.Events
{
    public class StudyCreated : IEvent
    {
        public StudyCreated(StudyRecord study, ApiResponse<Study> apiResponse)
        {
            Study = study;
            ApiResponse = apiResponse;
        }

        public StudyRecord Study { get; }
        public ApiResponse<Study> ApiResponse { get; }
    }

    public class StudyCreatedHandler : IEventHandler<StudyCreated>
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public StudyCreatedHandler(ICommandDispatcher commandDispatcher, ILogger<StudyCreatedHandler> logger)
        {
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(StudyCreated e, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Handling {nameof(StudyCreated)} event by dispatching {nameof(MarkAsHandled)} command.");

            await commandDispatcher.DispatchAsync(
                new MarkAsHandled(
                    (int)e.Study.EventId!,
                    e.ApiResponse.IsSuccessStatusCode(),
                    e.ApiResponse.Data?.Id,
                    e.ApiResponse.ValidationErrors.Any() ? e.ApiResponse.GetValidationErrorsAsSingleMessage() : e.ApiResponse.GetStatusMessage()),
                cancellationToken).WithoutCapturingContext();
        }
    }
}
