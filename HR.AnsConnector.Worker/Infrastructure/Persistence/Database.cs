using HR.AnsConnector.Features.Users;

namespace HR.AnsConnector.Infrastructure.Persistence
{
    public class Database : IDatabase
    {
        public Task<UserRecord?> GetNextUserAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task MarkAsHandledAsync(
            bool success, 
            string statusMessage, 
            string? errorMessage, 
            int? id, 
            int? eventId, 
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
