using HR.AnsConnector.Features.Common;
using HR.AnsConnector.Infrastructure;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Events;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Departments
{
    public class DepartmentUpdated : IEvent
    {
        public DepartmentUpdated(DepartmentRecord department, ApiResponse<Department> apiResponse)
        {
            StatusMessage = apiResponse.GetStatusMessage();
            Success = apiResponse.IsSuccessStatusCode();
            if (Success)
            {
                DepartmentId = apiResponse.Data!.Id;
            }
            else if (apiResponse.ValidationErrors.Any())
            {
                ErrorMessage = apiResponse.GetValidationErrorsAsSingleMessage();
            }
            EventId = department.EventId;
        }

        public bool Success { get; }
        public string StatusMessage { get; }
        public string? ErrorMessage { get; }
        public int? DepartmentId { get; }
        public int? EventId { get; }
    }

    public class DepartmentUpdatedHandler : IEventHandler<DepartmentUpdated>
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public DepartmentUpdatedHandler(ICommandDispatcher commandDispatcher, ILogger<DepartmentUpdatedHandler> logger)
        {
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(DepartmentUpdated e, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Handling {nameof(DepartmentUpdated)} event by dispatching {nameof(MarkAsHandled)} command.");

            await commandDispatcher.DispatchAsync(
                new MarkAsHandled(
                    e.Success,
                    e.StatusMessage,
                    e.ErrorMessage,
                    e.DepartmentId,
                    e.EventId),
                cancellationToken).WithoutCapturingContext();
        }
    }
}
