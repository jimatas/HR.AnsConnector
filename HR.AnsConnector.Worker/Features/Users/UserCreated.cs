using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;

namespace HR.AnsConnector.Features.Users
{
    public class UserCreated : IEvent
    {
        public UserCreated(User user)
        {
            User = user;
        }

        public User User { get; }
    }

    public class UserCreatedHandler : IEventHandler<UserCreated>
    {
        private readonly ICommandDispatcher commandDispatcher;

        public UserCreatedHandler(ICommandDispatcher commandDispatcher)
        {
            this.commandDispatcher = commandDispatcher;
        }

        public async Task HandleAsync(UserCreated e, CancellationToken cancellationToken)
        {
            // await commandDispatcher.DispatchAsync(new MarkAsHandled(), cancellationToken);
        }
    }
}
