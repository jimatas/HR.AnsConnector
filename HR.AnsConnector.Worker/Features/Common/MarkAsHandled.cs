using Developist.Core.Cqrs.Commands;

namespace HR.AnsConnector.Features.Common
{
    public class MarkAsHandled : ICommand
    {
        /// <summary>
        /// The generated id of the element in Ans.
        /// </summary>
        public int? Id { get; set; }
        public bool Success { get; set; }
        public int? EventId { get; set; }

        /// <summary>
        /// The HTTP status code and message of the API response. 
        /// </summary>
        public string? StatusInfo { get; set; }

        /// <summary>
        /// Any validation errors that were returned by the server; flattened to a single error message.
        /// </summary>
        public string? ErrorMessage { get; set; }
    }

    public class MarkAsHandledHandler : ICommandHandler<MarkAsHandled>
    {

        public Task HandleAsync(MarkAsHandled command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
