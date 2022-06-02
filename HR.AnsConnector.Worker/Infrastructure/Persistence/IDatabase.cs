using HR.AnsConnector.Features.Departments;
using HR.AnsConnector.Features.Studies;
using HR.AnsConnector.Features.Users;

namespace HR.AnsConnector.Infrastructure.Persistence
{
    /// <summary>
    /// Abstracts the source database and the stored procedures through which it is accessed.
    /// </summary>
    public interface IDatabase
    {
        Task<UserRecord?> GetNextUserAsync(CancellationToken cancellationToken = default);
        Task<DepartmentRecord?> GetNextDepartmentAsync(CancellationToken cancellationToken = default);
        Task<StudyRecord?> GetNextStudyAsync(CancellationToken cancellationToken = default);

        Task MarkAsHandledAsync(
            bool success,
            string? message,
            int? id,
            int? eventId,
            CancellationToken cancellationToken = default);
    }
}
