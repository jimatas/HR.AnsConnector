using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Utilities;

using HR.AnsConnector.Features.Common;
using HR.AnsConnector.Infrastructure;

namespace HR.AnsConnector.Features.Users
{
    public class UserCreated : IEvent
    {
        public UserCreated(ApiResponse<User> createUserResponse)
        {
            StatusMessage = createUserResponse.GetStatusMessage();
            Success = createUserResponse.IsSuccessStatusCode();
            if (Success)
            {
                UserId = createUserResponse.Data!.Id;
            }
            else
            {
                ErrorMessage = createUserResponse.GetValidationErrorsAsSingleMessage();
            }
        }

        public bool Success { get; }
        public string StatusMessage { get; }
        public string? ErrorMessage { get; }
        public int? UserId { get; }
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
                    eventId: null),
                cancellationToken).WithoutCapturingContext();
        }
    }
}
