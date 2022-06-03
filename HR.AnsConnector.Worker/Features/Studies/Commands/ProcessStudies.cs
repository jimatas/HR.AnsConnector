using HR.AnsConnector.Features.Studies.Queries;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Queries;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Studies.Commands
{
    public class ProcessStudies : ICommand
    {
        public ProcessStudies(int batchSize, bool isDeleteContext = false)
        {
            BatchSize = batchSize;
            IsDeleteContext = isDeleteContext;
        }

        public int BatchSize { get; set; }
        public bool IsDeleteContext { get; }
    }

    public class ProcessStudiesHandler : ICommandHandler<ProcessStudies>
    {
        private readonly IQueryDispatcher queryDispatcher;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public ProcessStudiesHandler(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, ILogger<ProcessStudiesHandler> logger)
        {
            this.queryDispatcher = queryDispatcher;
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(ProcessStudies command, CancellationToken cancellationToken)
        {
            int created = 0,
                updated = 0,
                deleted = 0;

            logger.LogInformation("Processing studies to {Action}.", command.IsDeleteContext ? "delete" : "create or update");

            for (var i = 0; i < command.BatchSize; i++)
            {
                var nextStudy = await queryDispatcher.DispatchAsync(new GetNextStudy(), cancellationToken).WithoutCapturingContext();
                if (nextStudy is null
                    || ((nextStudy.IsToBeCreated() || nextStudy.IsToBeUpdated()) && command.IsDeleteContext)
                    || (nextStudy.IsToBeDeleted() && !command.IsDeleteContext))
                {
                    logger.LogInformation("No more studies to {Action}. Ending batch run.", command.IsDeleteContext ? "delete" : "create or update");
                    break;
                }

                if (nextStudy.IsToBeCreated())
                {
                    await commandDispatcher.DispatchAsync(new CreateStudy(nextStudy), cancellationToken).WithoutCapturingContext();
                    created++;
                }
                else if (nextStudy.IsToBeUpdated())
                {
                    await commandDispatcher.DispatchAsync(new UpdateStudy(nextStudy), cancellationToken).WithoutCapturingContext();
                    updated++;
                }
                else if (nextStudy.IsToBeDeleted())
                {
                    await commandDispatcher.DispatchAsync(new DeleteStudy(nextStudy), cancellationToken).WithoutCapturingContext();
                    deleted++;
                }
            }

            logger.LogInformation("Processed {Processed} stud(y)(ies) in total.", created + updated + deleted);
            if (command.IsDeleteContext)
            {
                logger.LogDebug("Deleted {Deleted} studies.", deleted);
            }
            else
            {
                logger.LogDebug("Created {Created} stud(y)(ies).", created);
                logger.LogDebug("Updated {Updated} stud(y)(ies).", updated);
            }
        }
    }
}
