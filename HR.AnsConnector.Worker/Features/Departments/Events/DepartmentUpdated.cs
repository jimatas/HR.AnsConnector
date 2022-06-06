using HR.AnsConnector.Features.Common.Commands;
using HR.AnsConnector.Infrastructure;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Events;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Departments.Events
{
    public class DepartmentUpdated : IEvent
    {
        public DepartmentUpdated(DepartmentRecord department, ApiResponse<Department> apiResponse)
        {
            Department = department;
            ApiResponse = apiResponse;
        }

        public DepartmentRecord Department { get; }
        public ApiResponse<Department> ApiResponse { get; }
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
                    (int)e.Department.EventId!,
                    e.ApiResponse.IsSuccessStatusCode(),
                    e.ApiResponse.Data?.Id,
                    e.ApiResponse.ValidationErrors.Any() ? e.ApiResponse.GetValidationErrorsAsSingleMessage() : e.ApiResponse.GetStatusMessage()),
                cancellationToken).WithoutCapturingContext();
        }
    }
}
