using HR.AnsConnector.Features.Users.Queries;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Queries;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Users.Commands
{
    public class ProcessUsers : ICommand
    {
        public ProcessUsers(int batchSize, bool isDeleteContext = false)
        {
            BatchSize = batchSize;
            IsDeleteContext = isDeleteContext;
        }

        public int BatchSize { get; set; }
        public bool IsDeleteContext { get; }
    }

    public class ProcessUsersHandler : ICommandHandler<ProcessUsers>
    {
        private readonly IQueryDispatcher queryDispatcher;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public ProcessUsersHandler(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, ILogger<ProcessUsersHandler> logger)
        {
            this.queryDispatcher = queryDispatcher;
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(ProcessUsers command, CancellationToken cancellationToken)
        {
            int created = 0,
                updated = 0,
                deleted = 0;

            logger.LogInformation("Processing users to {Action}.", command.IsDeleteContext ? "delete" : "create or update");

            for (var i = 0; i < command.BatchSize; i++)
            {
                var nextUser = await queryDispatcher.DispatchAsync(new GetNextUser(), cancellationToken).WithoutCapturingContext();
                if (nextUser is null || ((nextUser.IsToBeCreated() || nextUser.IsToBeUpdated()) && command.IsDeleteContext) || (nextUser.IsToBeDeleted() && !command.IsDeleteContext))
                {
                    logger.LogInformation("No more users to {Action}. Ending batch run.", command.IsDeleteContext ? "delete" : "create or update");
                    break;
                }

                if (nextUser.IsToBeCreated())
                {
                    await commandDispatcher.DispatchAsync(new CreateUser(nextUser), cancellationToken).WithoutCapturingContext();
                    created++;
                }
                else if (nextUser.IsToBeUpdated())
                {
                    await commandDispatcher.DispatchAsync(new UpdateUser(nextUser), cancellationToken).WithoutCapturingContext();
                    updated++;
                }
                else if (nextUser.IsToBeDeleted())
                {
                    await commandDispatcher.DispatchAsync(new DeleteUser(nextUser), cancellationToken).WithoutCapturingContext();
                    deleted++;
                }
            }

            logger.LogInformation("Processed {Processed} user(s) in total.", created + updated + deleted);
            if (command.IsDeleteContext)
            {
                logger.LogDebug("Deleted {Deleted} user(s).", deleted);
            }
            else
            {
                logger.LogDebug("Created {Created} user(s).", created);
                logger.LogDebug("Updated {Updated} user(s).", updated);
            }
        }
    }
}
