using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Utilities;

using HR.AnsConnector.Infrastructure;

namespace HR.AnsConnector.Features.Users
{
    public class UpdateUser : ICommand
    {
        public UpdateUser(UserRecord user)
        {
            User = user;
        }

        public UserRecord User { get; }
    }

    public class UpdateUserHandler : ICommandHandler<UpdateUser>
    {
        private readonly IApiClient apiClient;
        private readonly IEventDispatcher eventDispatcher;
        private readonly ILogger logger;

        public UpdateUserHandler(IApiClient apiClient, IEventDispatcher eventDispatcher, ILogger<UpdateUserHandler> logger)
        {
            this.apiClient = apiClient;
            this.eventDispatcher = eventDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(UpdateUser command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Updating {User} in Ans.", command.User);

            var apiResponse = await apiClient.UpdateUserAsync(command.User, cancellationToken).WithoutCapturingContext();
            if (apiResponse.IsSuccessStatusCode())
            {
                logger.LogInformation("{User} was successfully updated in Ans.", command.User);
            }
            else if (apiResponse.IsErrorStatusCode())
            {
                logger.LogWarning("Received {StatusMessage} while attempting to update {User} in Ans. [{ValidationErrors}]",
                    apiResponse.GetStatusMessage(),
                    command.User,
                    apiResponse.GetValidationErrorsAsSingleMessage());
            }

            await eventDispatcher.DispatchAsync(new UserUpdated(command.User, apiResponse), cancellationToken).WithoutCapturingContext();
        }
    }
}
