using Developist.Core.Cqrs.Commands;

using HR.AnsConnector.Features.Users;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.AnsConnector.Worker.Features.Users
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
