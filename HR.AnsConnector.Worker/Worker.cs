using Developist.Core.Cqrs.Commands;
using Developist.Core.Utilities;

using HR.AnsConnector.Features.Users;

namespace HR.AnsConnector
{
    public class Worker : BackgroundService
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger<Worker> logger;

        public Worker(ICommandDispatcher commandDispatcher, ILogger<Worker> logger)
        {
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Starting new batch run.");

            await commandDispatcher.DispatchAsync(new ProcessUsers(), stoppingToken).WithoutCapturingContext();
            await commandDispatcher.DispatchAsync(new ProcessUsers { IsDeleteContext = true }, stoppingToken).WithoutCapturingContext();

            logger.LogInformation("Done running batch.");
        }
    }
}
