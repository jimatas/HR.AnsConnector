namespace HR.AnsConnector.Infrastructure.Persistence
{
    /// <summary>
    /// Abstracts the source database.
    /// </summary>
    public interface IDatabase
    {
        Task MarkAsHandledAsync(
            bool success,
            string statusMessage, 
            string? errorMessage, 
            int? id, 
            int? eventId,
            CancellationToken cancellationToken = default);
    }
}
