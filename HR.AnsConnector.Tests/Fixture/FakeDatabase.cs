﻿using HR.AnsConnector.Features.Departments;
using HR.AnsConnector.Features.Users;
using HR.AnsConnector.Infrastructure.Persistence;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HR.AnsConnector.Tests.Fixture
{
    public class FakeDatabase : IDatabase
    {
        public Queue<UserRecord> Users { get; } = new Queue<UserRecord>();
        public Queue<DepartmentRecord> Departments { get; } = new Queue<DepartmentRecord>();

        public Task<UserRecord?> GetNextUserAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(Users.TryPeek(out var user) ? user : null);

        public Task<DepartmentRecord?> GetNextDepartmentAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(Departments.TryPeek(out var department) ? department : null);

        public Task MarkAsHandledAsync(bool success, string? message, int? id, int? eventId, CancellationToken cancellationToken = default)
        {
            if (Users.TryPeek(out var user) && user.EventId == eventId)
            {
                Users.Dequeue();
            }
            else if (Departments.TryPeek(out var department) && department.EventId == eventId)
            {
                Departments.Dequeue();
            }
            else
            {
                throw new AssertFailedException($"Could not mark event {eventId} as handled. "
                    + $"{nameof(IDatabase)}.{nameof(MarkAsHandledAsync)} was not called in the right sequence or it was called at an inappropriate time.");
            }

            return Task.CompletedTask;
        }
    }
}