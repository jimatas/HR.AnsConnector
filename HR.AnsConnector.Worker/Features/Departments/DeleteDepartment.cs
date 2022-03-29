using HR.AnsConnector.Infrastructure;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Events;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Departments
{
    public class DeleteDepartment : ICommand
    {
        public DeleteDepartment(DepartmentRecord department)
        {
            Department = department;
        }

        public DepartmentRecord Department { get; }
    }

    public class DeleteDepartmentHandler : ICommandHandler<DeleteDepartment>
    {
        private readonly IApiClient apiClient;
        private readonly IEventDispatcher eventDispatcher;
        private readonly ILogger logger;

        public DeleteDepartmentHandler(IApiClient apiClient, IEventDispatcher eventDispatcher, ILogger<DeleteDepartmentHandler> logger)
        {
            this.apiClient = apiClient;
            this.eventDispatcher = eventDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(DeleteDepartment command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Deleting {Department} from Ans.", command.Department);

            var apiResponse = await apiClient.DeleteDepartmentAsync(command.Department, cancellationToken).WithoutCapturingContext();
            if (apiResponse.IsSuccessStatusCode())
            {
                logger.LogInformation("{Department} was successfully deleted from Ans.", command.Department);
            }
            else if (apiResponse.IsErrorStatusCode())
            {
                logger.LogWarning("Received {StatusMessage} while attempting to delete {Department} from Ans. [{ValidationErrors}]",
                    apiResponse.GetStatusMessage(),
                    command.Department,
                    apiResponse.GetValidationErrorsAsSingleMessage());
            }

            await eventDispatcher.DispatchAsync(new DepartmentDeleted(command.Department, apiResponse), cancellationToken).WithoutCapturingContext();
        }
    }
}
