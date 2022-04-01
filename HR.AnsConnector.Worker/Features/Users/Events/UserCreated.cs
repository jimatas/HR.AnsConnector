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
            StatusMessage = apiResponse.GetStatusMessage();
            Success = apiResponse.IsSuccessStatusCode();
            if (Success)
            {
                UserId = apiResponse.Data!.Id;
            }
            else if (apiResponse.ValidationErrors.Any())
            {
                ErrorMessage = apiResponse.GetValidationErrorsAsSingleMessage();
            }
            EventId = user.EventId;
        }

        public bool Success { get; }
        public string StatusMessage { get; }
        public string? ErrorMessage { get; }
        public int? UserId { get; }
        public int? EventId { get; }
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
                    e.Success,
                    e.StatusMessage,
                    e.ErrorMessage,
                    e.UserId,
                    e.EventId),
                cancellationToken).WithoutCapturingContext();
        }
    }
}
