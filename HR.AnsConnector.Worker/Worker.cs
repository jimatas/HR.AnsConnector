using HR.AnsConnector.Features.Departments.Commands;
using HR.AnsConnector.Features.Users.Commands;
using HR.AnsConnector.Infrastructure;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

using Microsoft.Extensions.Options;

namespace HR.AnsConnector
{
    public class Worker : BackgroundService
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger<Worker> logger;
        private readonly BatchSettings batchSettings;

        public Worker(ICommandDispatcher commandDispatcher, ILogger<Worker> logger, IOptions<BatchSettings> batchSettings)
        {
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
            this.batchSettings = batchSettings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var isDeleteContext = false;
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Starting new batch run.");

                await commandDispatcher.DispatchAsync(new ProcessUsers(batchSettings.BatchSize, isDeleteContext), stoppingToken).WithoutCapturingContext();
                await commandDispatcher.DispatchAsync(new ProcessDepartments(batchSettings.BatchSize, isDeleteContext), stoppingToken).WithoutCapturingContext();
                isDeleteContext = !isDeleteContext;

                logger.LogInformation("Done running batch.");

                await Task.Delay(batchSettings.TimeDelayBetweenRuns, stoppingToken).WithoutCapturingContext();
            }
        }
    }
}
