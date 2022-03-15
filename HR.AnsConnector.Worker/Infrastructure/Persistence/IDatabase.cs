using HR.AnsConnector.Features.Users;

namespace HR.AnsConnector.Infrastructure.Persistence
{
    /// <summary>
    /// Abstracts the source database.
    /// </summary>
    public interface IDatabase
    {
        Task<User?> GetNextUserAsync(CancellationToken cancellationToken = default);

        Task MarkAsHandledAsync(
            bool success,
            string statusMessage, 
            string? errorMessage, 
            int? id, 
            int? eventId,
            CancellationToken cancellationToken = default);
    }
}
