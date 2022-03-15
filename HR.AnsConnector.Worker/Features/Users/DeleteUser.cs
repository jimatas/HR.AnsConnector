using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Utilities;

using HR.AnsConnector.Infrastructure;

namespace HR.AnsConnector.Features.Users
{
    public class DeleteUser : ICommand
    {
        public DeleteUser(User user)
        {
            User = user;
        }

        public User User { get; }
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
