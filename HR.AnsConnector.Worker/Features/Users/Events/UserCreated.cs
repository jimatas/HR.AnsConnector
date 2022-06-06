using HR.AnsConnector.Features.Common.Commands;
using HR.AnsConnector.Infrastructure;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Events;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Users.Events
{
    public class UserCreated : IEvent
    {
        public UserCreated(UserRecord user, ApiResponse<User> apiResponse)
        {
            User = user;
            ApiResponse = apiResponse;
        }

        public UserRecord User { get; }
        public ApiResponse<User> ApiResponse { get; }
    }

    public class UserCreatedHandler : IEventHandler<UserCreated>
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public UserCreatedHandler(ICommandDispatcher commandDispatcher, ILogger<UserCreatedHandler> logger)
        {
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(UserCreated e, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Handling {nameof(UserCreated)} event by dispatching {nameof(MarkAsHandled)} command.");

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
