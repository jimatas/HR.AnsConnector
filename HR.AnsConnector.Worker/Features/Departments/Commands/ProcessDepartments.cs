using HR.AnsConnector.Features.Departments.Queries;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Queries;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Departments.Commands
{
    public class ProcessDepartments : ICommand
    {
        public ProcessDepartments(int batchSize, bool isDeleteContext = false)
        {
            BatchSize = batchSize;
            IsDeleteContext = isDeleteContext;
        }

        public int BatchSize { get; set; }
        public bool IsDeleteContext { get; }
    }

    public class ProcessDepartmentsHandler : ICommandHandler<ProcessDepartments>
    {
        private readonly IQueryDispatcher queryDispatcher;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public ProcessDepartmentsHandler(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, ILogger<ProcessDepartmentsHandler> logger)
        {
            this.queryDispatcher = queryDispatcher;
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(ProcessDepartments command, CancellationToken cancellationToken)
        {
            int created = 0,
                updated = 0,
                deleted = 0;

            for (var i = 0; i < command.BatchSize; i++)
            {
                var nextDepartment = await queryDispatcher.DispatchAsync(new GetNextDepartment(), cancellationToken).WithoutCapturingContext();
                if (nextDepartment is null
                    || ((nextDepartment.IsToBeCreated() || nextDepartment.IsToBeUpdated()) && command.IsDeleteContext)
                    || (nextDepartment.IsToBeDeleted() && !command.IsDeleteContext))
                {
                    logger.LogInformation("No more departments to {Action}. Ending batch run.", command.IsDeleteContext ? "delete" : "create or update");
                    break;
                }

                if (nextDepartment.IsToBeCreated())
                {
                    await commandDispatcher.DispatchAsync(new CreateDepartment(nextDepartment), cancellationToken).WithoutCapturingContext();
                    created++;
                }
                else if (nextDepartment.IsToBeUpdated())
                {
                    await commandDispatcher.DispatchAsync(new UpdateDepartment(nextDepartment), cancellationToken).WithoutCapturingContext();
                    updated++;
                }
                else if (nextDepartment.IsToBeDeleted())
                {
                    await commandDispatcher.DispatchAsync(new DeleteDepartment(nextDepartment), cancellationToken).WithoutCapturingContext();
                    deleted++;
                }
                logger.LogInformation("Processed {Processed} department(s) in total.", created + updated + deleted);

                logger.LogDebug("Created {Created} department(s).", created);
                logger.LogDebug("Updated {Updated} department(s).", updated);
                logger.LogDebug("Deleted {Deleted} department(s).", deleted);
            }
        }
    }
}
