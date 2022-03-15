using Developist.Core.Cqrs.Commands;

namespace HR.AnsConnector.Features.Common
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
        /// The HTTP status code and description of the server response. 
        /// </summary>
        public string StatusMessage { get; }

        /// <summary>
        /// Any validation errors that were returned by the server; flattened to a single error message.
        /// </summary>
        public string? ErrorMessage { get; }

        /// <summary>
        /// The generated id of the element in Ans, if successful.
        /// </summary>
        public int? Id { get; }
        public int? EventId { get; }
    }

    public class MarkAsHandledHandler : ICommandHandler<MarkAsHandled>
    {
        public Task HandleAsync(MarkAsHandled command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
