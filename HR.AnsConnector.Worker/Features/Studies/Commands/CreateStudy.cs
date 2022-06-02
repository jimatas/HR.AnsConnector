using HR.AnsConnector.Features.Studies.Events;
using HR.AnsConnector.Infrastructure;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Events;
using HR.Common.Utilities;

namespace HR.AnsConnector.Features.Studies.Commands
{
    public class CreateStudy : ICommand
    {
        public CreateStudy(StudyRecord study)
        {
            Study = study;
        }

        public StudyRecord Study { get; }
    }

    public class CreateStudyHandler : ICommandHandler<CreateStudy>
    {
        private readonly IApiClient apiClient;
        private readonly IEventDispatcher eventDispatcher;
        private readonly ILogger logger;

        public CreateStudyHandler(IApiClient apiClient, IEventDispatcher eventDispatcher, ILogger<CreateStudyHandler> logger)
        {
            this.apiClient = apiClient;
            this.eventDispatcher = eventDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(CreateStudy command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Creating {Study} in Ans.", command.Study);

            var apiResponse = await apiClient.CreateStudyAsync(command.Study, cancellationToken).WithoutCapturingContext();
            if (apiResponse.IsSuccessStatusCode())
            {
                logger.LogInformation("{Study} was successfully created in Ans.", command.Study);
            }
            else if (apiResponse.IsErrorStatusCode())
            {
                logger.LogWarning("Received {StatusMessage} while attempting to create {Study} in Ans. [{ValidationErrors}]",
                    apiResponse.GetStatusMessage(),
                    command.Study,
                    apiResponse.GetValidationErrorsAsSingleMessage());
            }

            await eventDispatcher.DispatchAsync(new StudyCreated(command.Study, apiResponse), cancellationToken).WithoutCapturingContext();
        }
    }
}
