using HR.AnsConnector.Features.Departments;
using HR.AnsConnector.Features.Users;
using HR.AnsConnector.Infrastructure.Persistence;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HR.AnsConnector.Tests.Fixture
{
    public class FakeDatabase : IDatabase
    {
        public Queue<UserRecord> Users { get; set; } = new Queue<UserRecord>();
        public Queue<DepartmentRecord> Departments { get; set; } = new Queue<DepartmentRecord>();

        public Task<UserRecord?> GetNextUserAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(Users.Any() ? Users.Peek() : null);

        public Task<DepartmentRecord?> GetNextDepartmentAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<DepartmentRecord?>(Departments.Any() ? Departments.Peek() : null);

        public Task MarkAsHandledAsync(bool success, string? message, int? id, int? eventId, CancellationToken cancellationToken = default)
        {
            if (Users.Any() && Users.Peek().EventId == eventId)
            {
                Users.Dequeue();
            }
            else if (Departments.Any() && Departments.Peek().EventId == eventId)
            {
                Departments.Dequeue();
            }
            else
            {
                throw new AssertFailedException($"Could not mark event with id {eventId} as handled. "
                    + $"{nameof(IDatabase)}.{nameof(MarkAsHandledAsync)} was not called in the right sequence or it was called at an inappropriate time.");
            }

            return Task.CompletedTask;
        }
    }
}
