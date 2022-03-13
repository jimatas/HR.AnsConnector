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
}
