using HR.AnsConnector.Features.Common.Commands;
using HR.AnsConnector.Infrastructure;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Events;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Departments.Events
{
    public class DepartmentDeleted : IEvent
    {
        public DepartmentDeleted(DepartmentRecord department, ApiResponse<Department> apiResponse)
        {
            Department = department;
            ApiResponse = apiResponse;
        }

        public DepartmentRecord Department { get; }
        public ApiResponse<Department> ApiResponse { get; }
    }

    public class DepartmentDeletedHandler : IEventHandler<DepartmentDeleted>
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public DepartmentDeletedHandler(ICommandDispatcher commandDispatcher, ILogger<DepartmentDeletedHandler> logger)
        {
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(DepartmentDeleted e, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Handling {nameof(DepartmentDeleted)} event by dispatching {nameof(MarkAsHandled)} command.");

            await commandDispatcher.DispatchAsync(
                new MarkAsHandled(
                    e.ApiResponse.IsSuccessStatusCode(),
                    e.ApiResponse.GetStatusMessage(),
                    e.ApiResponse.GetValidationErrorsAsSingleMessage(),
                    e.ApiResponse.Data?.Id,
                    e.Department.EventId),
                cancellationToken).WithoutCapturingContext();
        }
    }
}
