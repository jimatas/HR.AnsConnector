using HR.AnsConnector.Features.Studies.Events;
using HR.AnsConnector.Infrastructure;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Events;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Studies.Commands
{
    public class UpdateStudy : ICommand
    {
        public UpdateStudy(StudyRecord study)
        {
            Study = study;
        }

        public StudyRecord Study { get; }
    }

    public class UpdateStudyHandler : ICommandHandler<UpdateStudy>
    {
        private readonly IApiClient apiClient;
        private readonly IEventDispatcher eventDispatcher;
        private readonly ILogger logger;

        public UpdateStudyHandler(IApiClient apiClient, IEventDispatcher eventDispatcher, ILogger<UpdateStudyHandler> logger)
        {
            this.apiClient = apiClient;
            this.eventDispatcher = eventDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(UpdateStudy command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Updating {Study} in Ans.", command.Study);

            var apiResponse = await apiClient.UpdateStudyAsync(command.Study, cancellationToken).WithoutCapturingContext();
            if (apiResponse.IsSuccessStatusCode())
            {
                logger.LogInformation("{Study} was successfully updated in Ans.", command.Study);
            }
            else if (apiResponse.IsErrorStatusCode())
            {
                logger.LogWarning("Received {StatusMessage} while attempting to update {Study} in Ans. [{ValidationErrors}]",
                    apiResponse.GetStatusMessage(),
                    command.Study,
                    apiResponse.GetValidationErrorsAsSingleMessage());
            }

            await eventDispatcher.DispatchAsync(new StudyUpdated(command.Study, apiResponse), cancellationToken).WithoutCapturingContext();
        }
    }
}
