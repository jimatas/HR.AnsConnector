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
            StatusMessage = apiResponse.GetStatusMessage();
            Success = apiResponse.IsSuccessStatusCode();
            if (Success)
            {
                StudyId = apiResponse.Data!.Id;
            }
            else if (apiResponse.ValidationErrors.Any())
            {
                ErrorMessage = apiResponse.GetValidationErrorsAsSingleMessage();
            }
            EventId = study.EventId;
        }

        public bool Success { get; }
        public string StatusMessage { get; }
        public string? ErrorMessage { get; }
        public int? StudyId { get; }
        public int? EventId { get; }
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
                    e.Success,
                    e.StatusMessage,
                    e.ErrorMessage,
                    e.StudyId,
                    e.EventId),
                cancellationToken).WithoutCapturingContext();
        }
    }
}
