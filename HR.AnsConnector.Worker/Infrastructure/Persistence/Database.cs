using HR.AnsConnector.Features.Users;

namespace HR.AnsConnector.Infrastructure.Persistence
{
    public class Database : IDatabase
    {
        public Task<UserRecord?> GetNextUserAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<UserRecord?>(new()
            {
                Email = "atask@hr.nl",
                FirstName = "Jim",
                LastName = "Atas",
                Role = UserRole.Staff,
                UniqueId = "atask@hro.nl",
                ExternalId = "atask",
                EventId = 1,
                Action = "c"
            });
        }

        public Task MarkAsHandledAsync(
            bool success, 
            string statusMessage, 
            string? errorMessage, 
            int? id, 
            int? eventId, 
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
