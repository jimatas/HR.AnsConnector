﻿using HR.AnsConnector.Features.Departments.Events;
using HR.AnsConnector.Features.Users.Events;
using HR.Common.Cqrs.Events;

using System.Threading;
using System.Threading.Tasks;

namespace HR.AnsConnector.Tests.Fixture
{
    public class EventHandlerSpy : 
        IEventHandler<UserCreated>, IEventHandler<UserUpdated>, IEventHandler<UserDeleted>,
        IEventHandler<DepartmentCreated>, IEventHandler<DepartmentUpdated>, IEventHandler<DepartmentDeleted>
    {
        public bool IsUserCreatedCalled { get; private set; }
        public bool IsUserUpdatedCalled { get; private set; }
        public bool IsUserDeletedCalled { get; private set; }

        public bool IsDepartmentCreatedCalled { get; private set; }
        public bool IsDepartmentUpdatedCalled { get; private set; }
        public bool IsDepartmentDeletedCalled { get; private set; }

        public Task HandleAsync(UserCreated e, CancellationToken cancellationToken)
        {
            IsUserCreatedCalled = true;
            return Task.CompletedTask;
        }

        public Task HandleAsync(UserUpdated e, CancellationToken cancellationToken)
        {
            IsUserUpdatedCalled = true;
            return Task.CompletedTask;
        }

        public Task HandleAsync(UserDeleted e, CancellationToken cancellationToken)
        {
            IsUserDeletedCalled = true;
            return Task.CompletedTask;
        }

        public Task HandleAsync(DepartmentCreated e, CancellationToken cancellationToken)
        {
            IsDepartmentCreatedCalled = true;
            return Task.CompletedTask;
        }

        public Task HandleAsync(DepartmentUpdated e, CancellationToken cancellationToken)
        {
            IsDepartmentUpdatedCalled = true;
            return Task.CompletedTask;
        }

        public Task HandleAsync(DepartmentDeleted e, CancellationToken cancellationToken)
        {
            IsDepartmentDeletedCalled = true;
            return Task.CompletedTask;
        }
    }
}