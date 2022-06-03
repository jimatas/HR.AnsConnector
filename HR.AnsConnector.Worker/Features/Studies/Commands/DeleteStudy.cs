using HR.AnsConnector.Features.Studies.Events;
using HR.AnsConnector.Infrastructure;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Events;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Studies.Commands
{
    public class DeleteStudy : ICommand
    {
        public DeleteStudy(StudyRecord study)
        {
            Study = study;
        }

        public StudyRecord Study { get; }
    }

    public class DeleteStudyHandler : ICommandHandler<DeleteStudy>
    {
        private readonly IApiClient apiClient;
        private readonly IEventDispatcher eventDispatcher;
        private readonly ILogger logger;

        public DeleteStudyHandler(IApiClient apiClient, IEventDispatcher eventDispatcher, ILogger<DeleteStudyHandler> logger)
        {
            this.apiClient = apiClient;
            this.eventDispatcher = eventDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(DeleteStudy command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Deleting {Study} from Ans.", command.Study);

            var apiResponse = await apiClient.DeleteStudyAsync(command.Study, cancellationToken).WithoutCapturingContext();
            if (apiResponse.IsSuccessStatusCode())
            {
                logger.LogInformation("{Study} was successfully deleted from Ans.", command.Study);
            }
            else if (apiResponse.IsErrorStatusCode())
            {
                logger.LogWarning("Received {StatusMessage} while attempting to delete {Study} from Ans. [{ValidationErrors}]",
                    apiResponse.GetStatusMessage(),
                    command.Study,
                    apiResponse.GetValidationErrorsAsSingleMessage());
            }

            await eventDispatcher.DispatchAsync(new StudyDeleted(command.Study, apiResponse), cancellationToken).WithoutCapturingContext();
        }
    }
}
