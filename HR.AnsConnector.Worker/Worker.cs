using Developist.Core.Cqrs.Commands;
using Developist.Core.Utilities;

using HR.AnsConnector.Features.Users;
using HR.AnsConnector.Infrastructure;

namespace HR.AnsConnector
{
    public class Worker : BackgroundService
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly IApiClient apiClient;
        private readonly ILogger<Worker> logger;

        public Worker(ICommandDispatcher commandDispatcher, IApiClient apiClient, ILogger<Worker> logger)
        {
            this.commandDispatcher = commandDispatcher;
            this.apiClient = apiClient;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                UserRecord user = new()
                {
                    Email = "atask@hr.nl",
                    FirstName = "Jim",
                    LastName = "Atas",
                    Role = UserRole.Staff,
                    UniqueId = "atask@hro.nl",
                    ExternalId = "atask",
                    EventId = 1,
                    Action = "c"
                };

                await commandDispatcher.DispatchAsync(new CreateUser(user), stoppingToken);
                return;

                var getUsersResponse = await apiClient.SearchUsersAsync(new UserSearchCriteria
                {
                    ExternalId = "atask"
                }, stoppingToken).WithoutCapturingContext();

                if (getUsersResponse.IsSuccessStatusCode() && getUsersResponse.Data!.Any())
                {
                    var userToDelete = getUsersResponse.Data!.First();
                    var deleteUserResponse = await apiClient.DeleteUserAsync(userToDelete, stoppingToken).WithoutCapturingContext();
                }

                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
