using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Utilities;

using HR.AnsConnector.Features.Common;
using HR.AnsConnector.Infrastructure;

namespace HR.AnsConnector.Features.Users
{
    public class UserUpdated : IEvent
    {
        public UserUpdated(UserRecord user, ApiResponse<User> apiResponse)
        {
            StatusMessage = apiResponse.GetStatusMessage();
            Success = apiResponse.IsSuccessStatusCode();
            if (Success)
            {
                UserId = apiResponse.Data!.Id;
            }
            else
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
                    e.Success,
                    e.StatusMessage,
                    e.ErrorMessage,
                    e.UserId,
                    e.EventId),
                cancellationToken).WithoutCapturingContext();
        }
    }
}
