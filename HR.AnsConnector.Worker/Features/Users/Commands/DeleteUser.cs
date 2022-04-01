using HR.AnsConnector.Features.Users.Events;
using HR.AnsConnector.Infrastructure;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Events;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Users.Commands
{
    public class DeleteUser : ICommand
    {
        public DeleteUser(UserRecord user)
        {
            User = user;
        }

        public UserRecord User { get; }
    }

    public class DeleteUserHandler : ICommandHandler<DeleteUser>
    {
        private readonly IApiClient apiClient;
        private readonly IEventDispatcher eventDispatcher;
        private readonly ILogger logger;

        public DeleteUserHandler(IApiClient apiClient, IEventDispatcher eventDispatcher, ILogger<DeleteUserHandler> logger)
        {
            this.apiClient = apiClient;
            this.eventDispatcher = eventDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(DeleteUser command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Deleting {User} from Ans.", command.User);

            var apiResponse = await apiClient.DeleteUserAsync(command.User, cancellationToken).WithoutCapturingContext();
            if (apiResponse.IsSuccessStatusCode())
            {
                logger.LogInformation("{User} was successfully deleted from Ans.", command.User);
            }
            else if (apiResponse.IsErrorStatusCode())
            {
                logger.LogWarning("Received {StatusMessage} while attempting to delete {User} from Ans. [{ValidationErrors}]",
                    apiResponse.GetStatusMessage(),
                    command.User,
                    apiResponse.GetValidationErrorsAsSingleMessage());
            }

            await eventDispatcher.DispatchAsync(new UserDeleted(command.User, apiResponse), cancellationToken).WithoutCapturingContext();
        }
    }
}
