using Developist.Core.Cqrs.Queries;
using Developist.Core.Utilities;

using HR.AnsConnector.Infrastructure.Persistence;

namespace HR.AnsConnector.Features.Users
{
    public class GetNextUser : IQuery<User?>
    {
    }

    public class GetNextUserHandler : IQueryHandler<GetNextUser, User?>
    {
        private readonly IDatabase database;
        private readonly ILogger logger;

        public GetNextUserHandler(IDatabase database, ILogger<GetNextUserHandler> logger)
        {
            this.database = database;
            this.logger = logger;
        }

        public async Task<User?> HandleAsync(GetNextUser query, CancellationToken cancellationToken)
        {
            var user = await database.GetNextUserAsync(cancellationToken).WithoutCapturingContext();
            if (user is not null)
            {
                logger.LogInformation("Retrieved {User} from database.", user);
            }

            return user;
        }
    }
}
