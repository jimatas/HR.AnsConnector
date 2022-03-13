using HR.AnsConnector.Features.Users;
using HR.AnsConnector.Infrastructure;

namespace HR.AnsConnector
{
    public class Worker : BackgroundService
    {
        private readonly IApiClient apiClient;
        private readonly ILogger<Worker> logger;

        public Worker(ILogger<Worker> logger, IApiClient apiClient)
        {
            this.apiClient = apiClient;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var getUsersResponse = await apiClient.SearchUsersAsync(new UserSearchCriteria
                {
                    Email = "fit-toetsen@hr.nl",
                    StudentNumber = "9999999"
                }, stoppingToken);

                if (getUsersResponse.IsSuccessStatusCode() && getUsersResponse.Data!.Any())
                {
                    var userToDelete = getUsersResponse.Data!.First();
                    userToDelete.Id = 100;
                    var deleteUserResponse = await apiClient.DeleteUserAsync(userToDelete, stoppingToken);
                }

                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
