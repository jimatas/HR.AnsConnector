using Developist.Core.Cqrs.Commands;

namespace HR.AnsConnector.Features.Users
{
    public class CreateUser : ICommand
    {
        public CreateUser(User user)
        {
            User = user;
        }

        public User User { get; }
    }

    public class CreateUserHandler : ICommandHandler<CreateUser>
    {
        private readonly IApiClient apiClient;

        public CreateUserHandler(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public Task HandleAsync(CreateUser command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
