using HR.AnsConnector.Features.Departments.Events;
using HR.AnsConnector.Infrastructure;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Events;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Departments.Commands
{
    public class CreateDepartment : ICommand
    {
        public CreateDepartment(DepartmentRecord department)
        {
            Department = department;
        }

        public DepartmentRecord Department { get; }
    }

    public class CreateDepartmentHandler : ICommandHandler<CreateDepartment>
    {
        private readonly IApiClient apiClient;
        private readonly IEventDispatcher eventDispatcher;
        private readonly ILogger logger;

        public CreateDepartmentHandler(IApiClient apiClient, IEventDispatcher eventDispatcher, ILogger<CreateDepartmentHandler> logger)
        {
            this.apiClient = apiClient;
            this.eventDispatcher = eventDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(CreateDepartment command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Creating {Department} in Ans.", command.Department);

            var apiResponse = await apiClient.CreateDepartmentAsync(command.Department, cancellationToken).WithoutCapturingContext();
            if (apiResponse.IsSuccessStatusCode())
            {
                logger.LogInformation("{Department} was successfully created in Ans.", command.Department);
            }
            else if (apiResponse.IsErrorStatusCode())
            {
                logger.LogWarning("Received {StatusMessage} while attempting to create {Department} in Ans. [{ValidationErrors}]",
                    apiResponse.GetStatusMessage(),
                    command.Department,
                    apiResponse.GetValidationErrorsAsSingleMessage());
            }

            await eventDispatcher.DispatchAsync(new DepartmentCreated(command.Department, apiResponse), cancellationToken).WithoutCapturingContext();
        }
    }
}
