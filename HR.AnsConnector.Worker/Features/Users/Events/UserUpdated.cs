using HR.AnsConnector.Features.Common.Commands;
using HR.AnsConnector.Infrastructure;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Events;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Users.Events
{
    public class UserUpdated : IEvent
    {
        public UserUpdated(UserRecord user, ApiResponse<User> apiResponse)
        {
            User = user;
            ApiResponse = apiResponse;
        }

        public UserRecord User { get; }
        public ApiResponse<User> ApiResponse { get; }
    }

    public class UserUpdatedHandler : IEventHandler<UserUpdated>
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public UserUpdatedHandler(ICommandDispatcher commandDispatcher, ILogger<UserUpdatedHandler> logger)
        {
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(UserUpdated e, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Handling {nameof(UserUpdated)} event by dispatching {nameof(MarkAsHandled)} command.");

            await commandDispatcher.DispatchAsync(
                new MarkAsHandled(
                    (int)e.User.EventId!,
                    e.ApiResponse.IsSuccessStatusCode(),
                    e.ApiResponse.Data?.Id,
                    e.ApiResponse.ValidationErrors.Any() ? e.ApiResponse.GetValidationErrorsAsSingleMessage() : e.ApiResponse.GetStatusMessage()),
                cancellationToken).WithoutCapturingContext();
        }
    }
}
