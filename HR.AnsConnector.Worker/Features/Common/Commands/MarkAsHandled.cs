using HR.AnsConnector.Infrastructure.Persistence;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Common.Commands
{
    public class MarkAsHandled : ICommand
    {
        public MarkAsHandled(int eventId, bool success, int? id, string? message)
        {
            EventId = eventId;
            Success = success;
            Id = id;
            Message = message;
        }

        public static MarkAsHandled Successfully(int eventId, int id) => new(eventId, success: true, id, message: string.Empty);
        public static MarkAsHandled Unsuccessfully(int eventId, string message) => new(eventId, success: false, id: null, message);
        public static MarkAsHandled Unsuccessfully(int eventId, string statusMessage, string? errorMessage = null) => new(eventId, success: false, id: null, string.IsNullOrEmpty(errorMessage) ? statusMessage : errorMessage);

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
        public string? Message { get; }
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
