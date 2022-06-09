using HR.AnsConnector.Features.Courses.Commands;
using HR.AnsConnector.Features.Departments.Commands;
using HR.AnsConnector.Features.Studies.Commands;
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
        private readonly BatchSettings batchSettings;
        private readonly ILogger logger;

        public Worker(ICommandDispatcher commandDispatcher, IOptions<BatchSettings> batchSettings, ILogger<Worker> logger)
        {
            this.commandDispatcher = commandDispatcher;
            this.batchSettings = batchSettings.Value;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var isDeleteContext = false;
                while (!stoppingToken.IsCancellationRequested)
                {
                    logger.LogInformation("Starting new batch run.");

                    await commandDispatcher.DispatchAsync(new ProcessUsers(batchSettings.BatchSize, isDeleteContext), stoppingToken).WithoutCapturingContext();
                    await commandDispatcher.DispatchAsync(new ProcessDepartments(batchSettings.BatchSize, isDeleteContext), stoppingToken).WithoutCapturingContext();
                    await commandDispatcher.DispatchAsync(new ProcessStudies(batchSettings.BatchSize, isDeleteContext), stoppingToken).WithoutCapturingContext();
                    await commandDispatcher.DispatchAsync(new ProcessCourses(batchSettings.BatchSize, isDeleteContext), stoppingToken).WithoutCapturingContext();
                    isDeleteContext = !isDeleteContext;

                    logger.LogInformation("Done running batch.");

                    await Task.Delay(batchSettings.TimeDelayBetweenRuns, stoppingToken).WithoutCapturingContext();
                }
            }
            catch (TaskCanceledException)
            {
                logger.LogWarning("The BackgroundService is stopping because a task was canceled.");
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "The BackgroundService failed.");

                Environment.Exit(ex.HResult);
            }
        }
    }
}
