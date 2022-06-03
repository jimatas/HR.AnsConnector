using HR.AnsConnector.Features.Common.Commands;
using HR.AnsConnector.Infrastructure;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Events;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Departments.Events
{
    public class DepartmentCreated : IEvent
    {
        public DepartmentCreated(DepartmentRecord department, ApiResponse<Department> apiResponse)
        {
            Department = department;
            ApiResponse = apiResponse;
        }

        public DepartmentRecord Department { get; }
        public ApiResponse<Department> ApiResponse { get; }
    }

    public class DepartmentCreatedHandler : IEventHandler<DepartmentCreated>
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public DepartmentCreatedHandler(ICommandDispatcher commandDispatcher, ILogger<DepartmentCreatedHandler> logger)
        {
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(DepartmentCreated e, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Handling {nameof(DepartmentCreated)} event by dispatching {nameof(MarkAsHandled)} command.");

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
