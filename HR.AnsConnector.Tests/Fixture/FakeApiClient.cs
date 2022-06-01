using HR.AnsConnector.Features.Departments;
using HR.AnsConnector.Features.Studies;
using HR.AnsConnector.Features.Users;
using HR.AnsConnector.Infrastructure;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HR.AnsConnector.Tests.Fixture
{
    public class FakeApiClient : IApiClient
    {
        public ApiResponse? LastApiResponse { get; private set; }
        public Queue<ApiResponse> ExpectedApiResponses { get; } = new Queue<ApiResponse>();

        #region Users
        public Task<ApiResponse<User>> CreateUserAsync(User user, CancellationToken cancellationToken = default)
            => Task.FromResult((ApiResponse<User>)(LastApiResponse = ExpectedApiResponses.Dequeue()));

        public Task<ApiResponse<User>> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
            => Task.FromResult((ApiResponse<User>)(LastApiResponse = ExpectedApiResponses.Dequeue()));

        public Task<ApiResponse<User>> DeleteUserAsync(User user, CancellationToken cancellationToken = default)
            => Task.FromResult((ApiResponse<User>)(LastApiResponse = ExpectedApiResponses.Dequeue()));

        public Task<ApiResponse<IEnumerable<User>>> SearchUsersAsync(UserSearchCriteria criteria, CancellationToken cancellationToken = default)
            => Task.FromResult((ApiResponse<IEnumerable<User>>)(LastApiResponse = ExpectedApiResponses.Dequeue()));
        #endregion

        #region Departments
        public Task<ApiResponse<Department>> CreateDepartmentAsync(Department department, CancellationToken cancellationToken = default)
            => Task.FromResult((ApiResponse<Department>)(LastApiResponse = ExpectedApiResponses.Dequeue()));

        public Task<ApiResponse<Department>> UpdateDepartmentAsync(Department department, CancellationToken cancellationToken = default)
            => Task.FromResult((ApiResponse<Department>)(LastApiResponse = ExpectedApiResponses.Dequeue()));

        public Task<ApiResponse<Department>> DeleteDepartmentAsync(Department department, CancellationToken cancellationToken = default)
            => Task.FromResult((ApiResponse<Department>)(LastApiResponse = ExpectedApiResponses.Dequeue()));

        public Task<ApiResponse<IEnumerable<Department>>> ListDepartmentsAsync(CancellationToken cancellationToken = default)
            => Task.FromResult((ApiResponse<IEnumerable<Department>>)(LastApiResponse = ExpectedApiResponses.Dequeue()));
        #endregion

        #region Studies
        public Task<ApiResponse<Study>> CreateStudyAsync(Study study, CancellationToken cancellationToken = default)
            => Task.FromResult((ApiResponse<Study>)(LastApiResponse = ExpectedApiResponses.Dequeue()));

        public Task<ApiResponse<IEnumerable<Study>>> ListStudiesAsync(int departmentId, CancellationToken cancellationToken = default)
            => Task.FromResult((ApiResponse<IEnumerable<Study>>)(LastApiResponse = ExpectedApiResponses.Dequeue()));
        #endregion
    }
}
