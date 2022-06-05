using HR.AnsConnector.Infrastructure.Persistence;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Common.Commands
{
    public class MarkAsHandled : ICommand
    {
        public MarkAsHandled(
            bool success,
            string statusMessage,
            string? errorMessage,
            int? id,
            int? eventId) : this(success, string.IsNullOrEmpty(errorMessage) ? statusMessage : errorMessage, id, eventId) { }

        public MarkAsHandled(bool success, string message, int? id, int? eventId)
        {
            Success = success;
            Message = message;
            Id = id;
            EventId = (int)eventId!;
        }

        public int EventId { get; }
        public bool Success { get; }

        /// <summary>
        /// The unique id of the element in Ans.
        /// Its value is generated upon successful creation.
        /// </summary>
        public int? Id { get; }

        /// <summary>
        /// Either any validation errors that were returned by the server, flattened to a single error message 
        /// -or- 
        /// the HTTP status code and description of the response.
        /// </summary>
        public string Message { get; }
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
                command.EventId,
                command.Success,
                command.Id,
                command.Success ? null : command.Message,
                cancellationToken).WithoutCapturingContext();

            logger.LogInformation("Marked object with Id {Id} and EventId {EventId} as handled in database.",
                command.Id?.ToString() ?? "[n/a]",
                command.EventId);
        }
    }
}
