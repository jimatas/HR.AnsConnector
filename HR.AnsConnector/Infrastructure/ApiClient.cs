using HR.AnsConnector.Features.Courses;
using HR.AnsConnector.Features.Departments;
using HR.AnsConnector.Features.Studies;
using HR.AnsConnector.Features.Users;
using HR.Common.Utilities;

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

        #region Users
        /// <inheritdoc/>
        public async Task<ApiResponse<User>> CreateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            var requestUri = $"schools/{apiSettings.TenantId}/users";
            using var httpResponse = await httpClient.PostAsync(requestUri, JsonContent.Create(user, mediaType: null, jsonOptions), cancellationToken).WithoutCapturingContext();

            return await httpResponse.ToApiResponseAsync<User>(jsonOptions, cancellationToken).WithoutCapturingContext();
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<User>> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            var requestUri = $"users/{user.Id}";
            using var httpResponse = await httpClient.PatchAsync(requestUri, JsonContent.Create(user, mediaType: null, jsonOptions), cancellationToken).WithoutCapturingContext();

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
            var requestUri = $"search/users?query={criteria.ToQueryString(jsonOptions.PropertyNamingPolicy)}";
            using var httpResponse = await httpClient.GetAsync(requestUri, cancellationToken).WithoutCapturingContext();

            return await httpResponse.ToApiResponseAsync<IEnumerable<User>>(jsonOptions, cancellationToken).WithoutCapturingContext();
        }
        #endregion

        #region Departments
        /// <inheritdoc/>
        public async Task<ApiResponse<Department>> CreateDepartmentAsync(Department department, CancellationToken cancellationToken = default)
        {
            var requestUri = $"schools/{apiSettings.TenantId}/departments";
            using var httpResponse = await httpClient.PostAsync(requestUri, JsonContent.Create(department, mediaType: null, jsonOptions), cancellationToken).WithoutCapturingContext();

            return await httpResponse.ToApiResponseAsync<Department>(jsonOptions, cancellationToken).WithoutCapturingContext();
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<Department>> UpdateDepartmentAsync(Department department, CancellationToken cancellationToken = default)
        {
            var requestUri = $"departments/{department.Id}";
            using var httpResponse = await httpClient.PatchAsync(requestUri, JsonContent.Create(department, mediaType: null, jsonOptions), cancellationToken).WithoutCapturingContext();

            return await httpResponse.ToApiResponseAsync<Department>(jsonOptions, cancellationToken).WithoutCapturingContext();
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<Department>> DeleteDepartmentAsync(Department department, CancellationToken cancellationToken = default)
        {
            var requestUri = $"departments/{department.Id}";
            using var httpResponse = await httpClient.DeleteAsync(requestUri, cancellationToken).WithoutCapturingContext();

            return await httpResponse.ToApiResponseAsync<Department>(jsonOptions, cancellationToken).WithoutCapturingContext();
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<Department>>> ListDepartmentsAsync(CancellationToken cancellationToken = default)
        {
            var requestUri = $"schools/{apiSettings.TenantId}/departments";
            using var httpResponse = await httpClient.GetAsync(requestUri, cancellationToken).WithoutCapturingContext();

            return await httpResponse.ToApiResponseAsync<IEnumerable<Department>>(jsonOptions, cancellationToken).WithoutCapturingContext();
        }
        #endregion

        #region Studies
        /// <inheritdoc/>
        public async Task<ApiResponse<Study>> CreateStudyAsync(Study study, CancellationToken cancellationToken = default)
        {
            var requestUri = $"departments/{study.DepartmentId}/studies";
            using var httpResponse = await httpClient.PostAsync(requestUri, JsonContent.Create(study, mediaType: null, jsonOptions), cancellationToken).WithoutCapturingContext();

            return await httpResponse.ToApiResponseAsync<Study>(jsonOptions, cancellationToken).WithoutCapturingContext();
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<Study>> UpdateStudyAsync(Study study, CancellationToken cancellationToken = default)
        {
            var requestUri = $"studies/{study.Id}";
            using var httpResponse = await httpClient.PatchAsync(requestUri, JsonContent.Create(study, mediaType: null, jsonOptions), cancellationToken).WithoutCapturingContext();

            return await httpResponse.ToApiResponseAsync<Study>(jsonOptions, cancellationToken).WithoutCapturingContext();
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<Study>> DeleteStudyAsync(Study study, CancellationToken cancellationToken = default)
        {
            var requestUri = $"studies/{study.Id}";
            using var httpResponse = await httpClient.DeleteAsync(requestUri, cancellationToken).WithoutCapturingContext();

            return await httpResponse.ToApiResponseAsync<Study>(jsonOptions, cancellationToken).WithoutCapturingContext();
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<Study>>> ListStudiesAsync(int departmentId, CancellationToken cancellationToken = default)
        {
            var requestUri = $"departments/{departmentId}/studies";
            using var httpResponse = await httpClient.GetAsync(requestUri, cancellationToken).WithoutCapturingContext();

            return await httpResponse.ToApiResponseAsync<IEnumerable<Study>>(jsonOptions, cancellationToken).WithoutCapturingContext();
        }
        #endregion

        #region Courses
        /// <inheritdoc/>
        public async Task<ApiResponse<Course>> CreateCourseAsync(Course course, CancellationToken cancellationToken = default)
        {
            var requestUri = $"schools/{apiSettings.TenantId}/courses";
            using var httpResponse = await httpClient.PostAsync(requestUri, JsonContent.Create(course, mediaType: null, jsonOptions), cancellationToken).WithoutCapturingContext();

            return await httpResponse.ToApiResponseAsync<Course>(jsonOptions, cancellationToken).WithoutCapturingContext();
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<Course>>> ListCoursesAsync(CancellationToken cancellationToken = default)
        {
            var requestUri = $"schools/{apiSettings.TenantId}/courses";
            using var httpResponse = await httpClient.GetAsync(requestUri, cancellationToken).WithoutCapturingContext();

            return await httpResponse.ToApiResponseAsync<IEnumerable<Course>>(jsonOptions, cancellationToken).WithoutCapturingContext();
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<Course>> DeleteCourseAsync(Course course, CancellationToken cancellationToken = default)
        {
            var requestUri = $"courses/{course.Id}";
            using var httpResponse = await httpClient.DeleteAsync(requestUri, cancellationToken).WithoutCapturingContext();

            return await httpResponse.ToApiResponseAsync<Course>(jsonOptions, cancellationToken).WithoutCapturingContext();
        }
        #endregion
    }
}
