using HR.AnsConnector.Features.Departments.Events;
using HR.AnsConnector.Infrastructure;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Events;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Departments.Commands
{
    public class UpdateDepartment : ICommand
    {
        public UpdateDepartment(DepartmentRecord department)
        {
            Department = department;
        }

        public DepartmentRecord Department { get; }
    }

    public class UpdateDepartmentHandler : ICommandHandler<UpdateDepartment>
    {
        private readonly IApiClient apiClient;
        private readonly IEventDispatcher eventDispatcher;
        private readonly ILogger logger;

        public UpdateDepartmentHandler(IApiClient apiClient, IEventDispatcher eventDispatcher, ILogger<UpdateDepartmentHandler> logger)
        {
            this.apiClient = apiClient;
            this.eventDispatcher = eventDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(UpdateDepartment command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Updating {Department} in Ans.", command.Department);

            var apiResponse = await apiClient.UpdateDepartmentAsync(command.Department, cancellationToken).WithoutCapturingContext();
            if (apiResponse.IsSuccessStatusCode())
            {
                logger.LogInformation("{Department} was successfully updated in Ans.", command.Department);
            }
            else if (apiResponse.IsErrorStatusCode())
            {
                logger.LogWarning("Received {StatusMessage} while attempting to update {Department} in Ans. [{ValidationErrors}]",
                    apiResponse.GetStatusMessage(),
                    command.Department,
                    apiResponse.GetValidationErrorsAsSingleMessage());
            }

            await eventDispatcher.DispatchAsync(new DepartmentUpdated(command.Department, apiResponse), cancellationToken).WithoutCapturingContext();
        }
    }
}
