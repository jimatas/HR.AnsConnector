using HR.AnsConnector.Infrastructure.Persistence;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Common.Commands
{
    public class MarkAsHandled : ICommand
    {
        public MarkAsHandled(bool success, string statusMessage, string? errorMessage, int? id, int? eventId)
        {
            Success = success;
            StatusMessage = statusMessage;
            ErrorMessage = errorMessage;
            Id = id;
            EventId = eventId;
        }

        public bool Success { get; }

        /// <summary>
        /// The HTTP status code and description of the response. 
        /// </summary>
        public string StatusMessage { get; }

        /// <summary>
        /// Any validation errors that were returned by the server, flattened to a single error message.
        /// </summary>
        public string? ErrorMessage { get; }

        /// <summary>
        /// The unique id of the element in Ans.
        /// Its value is generated upon successful creation.
        /// </summary>
        public int? Id { get; }

        public int? EventId { get; }
    }

    public class MarkAsHandledHandler : ICommandHandler<MarkAsHandled>
    {
        private readonly IDatabase database;
        private readonly ILogger logger;

        public MarkAsHandledHandler(IDatabase database, ILogger<MarkAsHandledHandler> logger)
        {
            this.database = database;
            this.logger = logger;
        }

        public async Task HandleAsync(MarkAsHandled command, CancellationToken cancellationToken)
        {
            await database.MarkAsHandledAsync(
                command.Success,
                command.Success ? null : command.ErrorMessage ?? command.StatusMessage,
                command.Id,
                command.EventId,
                cancellationToken).WithoutCapturingContext();

            logger.LogInformation("Marked object with Id {Id} and EventId {EventId} as handled in database.",
                command.Id?.ToString() ?? "[n/a]",
                command.EventId?.ToString() ?? "[n/a]");
        }
    }
}
