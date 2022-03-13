using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Utilities;

using HR.AnsConnector.Infrastructure;

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
        private readonly IEventDispatcher eventDispatcher;
        private readonly ILogger logger;

        public CreateUserHandler(IApiClient apiClient, IEventDispatcher eventDispatcher, ILogger<CreateUserHandler> logger)
        {
            this.apiClient = apiClient;
            this.eventDispatcher = eventDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(CreateUser command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Creating {User} in Ans.", command.User);

            var apiResponse = await apiClient.CreateUserAsync(command.User, cancellationToken).WithoutCapturingContext();
            if (apiResponse.IsSuccessStatusCode())
            {
                logger.LogInformation("{User} was successfully created in Ans.", command.User);

                await eventDispatcher.DispatchAsync(new UserCreated(apiResponse.Data!), cancellationToken).WithoutCapturingContext();
            }
            else if (apiResponse.IsErrorStatusCode())
            {
                logger.LogWarning("Received HTTP {StatusCode} - {StatusMessage} while creating {User} in Ans. [{ValidationErrors}]",
                    command.User,
                    apiResponse.StatusCode,
                    apiResponse.StatusMessage,
                    apiResponse.GetValidationErrorsAsSingleMessage());
            }
        }
    }
}
