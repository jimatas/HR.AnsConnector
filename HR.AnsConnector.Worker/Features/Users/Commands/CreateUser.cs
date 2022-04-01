using HR.AnsConnector.Features.Users.Events;
using HR.AnsConnector.Infrastructure;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Events;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Users.Commands
{
    public class CreateUser : ICommand
    {
        public CreateUser(UserRecord user)
        {
            User = user;
        }

        public UserRecord User { get; }
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
                if (apiResponse.StatusCode == 201)
                {
                    logger.LogInformation("{User} was successfully created in Ans.", command.User);
                }
                else if (apiResponse.StatusCode == 200)
                {
                    logger.LogInformation("{User} already exists in Ans and was updated instead.", command.User);
                }
            }
            else if (apiResponse.IsErrorStatusCode())
            {
                logger.LogWarning("Received {StatusMessage} while attempting to create {User} in Ans. [{ValidationErrors}]",
                    apiResponse.GetStatusMessage(),
                    command.User,
                    apiResponse.GetValidationErrorsAsSingleMessage());
            }

            await eventDispatcher.DispatchAsync(new UserCreated(command.User, apiResponse), cancellationToken).WithoutCapturingContext();
        }
    }
}
