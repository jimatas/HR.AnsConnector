using HR.AnsConnector.Features.Common.Commands;
using HR.AnsConnector.Infrastructure;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Events;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Users.Events
{
    public class UserDeleted : IEvent
    {
        public UserDeleted(UserRecord user, ApiResponse<User> apiResponse)
        {
            User = user;
            ApiResponse = apiResponse;
        }

        public UserRecord User { get; }
        public ApiResponse<User> ApiResponse { get; }
    }

    public class UserDeletedHandler : IEventHandler<UserDeleted>
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public UserDeletedHandler(ICommandDispatcher commandDispatcher, ILogger<UserDeletedHandler> logger)
        {
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(UserDeleted e, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Handling {nameof(UserDeleted)} event by dispatching {nameof(MarkAsHandled)} command.");

            await commandDispatcher.DispatchAsync(
                new MarkAsHandled(
                    e.ApiResponse.IsSuccessStatusCode(),
                    e.ApiResponse.GetStatusMessage(),
                    e.ApiResponse.GetValidationErrorsAsSingleMessage(),
                    e.ApiResponse.Data?.Id,
                    e.User.EventId),
                cancellationToken).WithoutCapturingContext();
        }
    }
}
