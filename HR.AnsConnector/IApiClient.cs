using HR.AnsConnector.Features.Users;
using HR.AnsConnector.Infrastructure;

namespace HR.AnsConnector
{
    public interface IApiClient
    {
        /// <summary>
        /// Create a new user. 
        /// </summary>
        /// <remarks>
        /// If an existing user is found based on the email or student number, the existing user is returned with status 200. 
        /// Empty attributes are filled with the provided information, but existing attributes are not overwritten.
        /// </remarks>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ApiResponse<User>> CreateUserAsync(User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update the information of an existing user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ApiResponse<User>> UpdateUserAsync(User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Soft delete a user by clearing all attributes except student number.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ApiResponse<User>> DeleteUserAsync(User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Search user(s) based on some specified criteria.
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<User>>> SearchUsersAsync(UserSearchCriteria criteria, CancellationToken cancellationToken = default);
    }
}
