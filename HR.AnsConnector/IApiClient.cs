using HR.AnsConnector.Features.Courses;
using HR.AnsConnector.Features.Departments;
using HR.AnsConnector.Features.Studies;
using HR.AnsConnector.Features.Users;
using HR.AnsConnector.Infrastructure;

namespace HR.AnsConnector
{
    public interface IApiClient
    {
        #region Users
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
        #endregion

        #region Departments
        /// <summary>
        /// Create department.
        /// </summary>
        /// <param name="department"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ApiResponse<Department>> CreateDepartmentAsync(Department department, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update department.
        /// </summary>
        /// <param name="department"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ApiResponse<Department>> UpdateDepartmentAsync(Department department, CancellationToken cancellationToken = default);

        /// <summary>
        /// Hard delete a department.
        /// </summary>
        /// <param name="department"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ApiResponse<Department>> DeleteDepartmentAsync(Department department, CancellationToken cancellationToken = default);

        /// <summary>
        /// List departments.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<Department>>> ListDepartmentsAsync(CancellationToken cancellationToken = default);
        #endregion

        #region Studies
        /// <summary>
        /// Create study.
        /// </summary>
        /// <param name="study"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ApiResponse<Study>> CreateStudyAsync(Study study, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update study.
        /// </summary>
        /// <param name="study"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ApiResponse<Study>> UpdateStudyAsync(Study study, CancellationToken cancellationToken = default);

        /// <summary>
        /// Hard delete a study.
        /// </summary>
        /// <param name="study"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ApiResponse<Study>> DeleteStudyAsync(Study study, CancellationToken cancellationToken = default);

        /// <summary>
        /// List all studies of the department.
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<Study>>> ListStudiesAsync(int departmentId, CancellationToken cancellationToken = default);
        #endregion

        #region Courses
        /// <summary>
        /// Create course.
        /// </summary>
        /// <param name="course"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ApiResponse<Course>> CreateCourseAsync(Course course, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update course.
        /// </summary>
        /// <remarks>
        /// To update from one role to another, the user must first be removed from the current role list and with a new request added to the chosen role list.
        /// </remarks>
        /// <param name="course"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ApiResponse<Course>> UpdateCourseAsync(Course course, CancellationToken cancellationToken = default);

        /// <summary>
        /// Soft delete a course by moving it to removed courses.
        /// </summary>
        /// <param name="course"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ApiResponse<Course>> DeleteCourseAsync(Course course, CancellationToken cancellationToken = default);

        /// <summary>
        /// List courses.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<Course>>> ListCoursesAsync(CancellationToken cancellationToken = default);
        #endregion
    }
}
