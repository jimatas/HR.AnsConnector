using Developist.Core.Utilities;

using HR.AnsConnector.Features.Users;

using Microsoft.Extensions.Options;

using System.Net.Http.Json;
using System.Text.Json;

namespace HR.AnsConnector.Infrastructure
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient httpClient;
        private readonly JsonSerializerOptions jsonOptions;
        private readonly ApiSettings apiSettings;

        public ApiClient(HttpClient httpClient, IOptions<ApiSettings> apiSettings, IOptions<JsonSerializerOptions> jsonOptions)
        {
            this.httpClient = httpClient;
            this.apiSettings = apiSettings.Value;
            this.jsonOptions = jsonOptions.Value;
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<User>> CreateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            var requestUri = $"schools/{apiSettings.TenantId}/users";
            using var httpResponse = await httpClient.PostAsync(requestUri, JsonContent.Create(user, options: jsonOptions), cancellationToken).WithoutCapturingContext();

            return await httpResponse.ToApiResponseAsync<User>(jsonOptions, cancellationToken).WithoutCapturingContext();
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<User>> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            var requestUri = $"schools/{apiSettings.TenantId}/users";
            using var httpResponse = await httpClient.PatchAsync(requestUri, JsonContent.Create(user, options: jsonOptions), cancellationToken).WithoutCapturingContext();

            return await httpResponse.ToApiResponseAsync<User>(jsonOptions, cancellationToken).WithoutCapturingContext();
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<User>> DeleteUserAsync(User user, CancellationToken cancellationToken = default)
        {
            var requestUri = $"users/{user.Id}";
            using var httpResponse = await httpClient.DeleteAsync(requestUri, cancellationToken).WithoutCapturingContext();

            return await httpResponse.ToApiResponseAsync<User>(jsonOptions, cancellationToken).WithoutCapturingContext();
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<User>>> SearchUsersAsync(UserSearchCriteria criteria, CancellationToken cancellationToken = default)
        {
            var requestUri = $"search/users?query={criteria.ToQueryString(jsonOptions)}";
            using var httpResponse = await httpClient.GetAsync(requestUri, cancellationToken).WithoutCapturingContext();

            return await httpResponse.ToApiResponseAsync<IEnumerable<User>>(jsonOptions, cancellationToken).WithoutCapturingContext();
        }
    }
}
