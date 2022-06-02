using HR.AnsConnector.Infrastructure.Persistence;
using HR.Common.Cqrs.Queries;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Studies.Queries
{
    public class GetNextStudy : IQuery<StudyRecord?>
    {
    }

    public class GetNextStudyHandler : IQueryHandler<GetNextStudy, StudyRecord?>
    {
        private readonly IDatabase database;
        private readonly ILogger logger;

        public GetNextStudyHandler(IDatabase database, ILogger<GetNextStudyHandler> logger)
        {
            this.database = database;
            this.logger = logger;
        }

        public async Task<StudyRecord?> HandleAsync(GetNextStudy query, CancellationToken cancellationToken)
        {
            var study = await database.GetNextStudyAsync(cancellationToken).WithoutCapturingContext();
            if (study is not null)
            {
                logger.LogInformation("Retrieved {Study} from database.", study);
            }

            return study;
        }
    }
}
