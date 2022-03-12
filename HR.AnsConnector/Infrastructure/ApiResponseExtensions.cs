namespace HR.AnsConnector.Infrastructure
{
    public static class ApiResponseExtensions
    {
        /// <summary>
        /// Determines whether the status code of the response indicates success.
        /// </summary>
        /// <param name="apiResponse"></param>
        /// <returns></returns>
        public static bool IsSuccessStatusCode(this ApiResponse apiResponse) => apiResponse.StatusCode >= 200 && apiResponse.StatusCode <= 299;

        /// <summary>
        /// Determines whether the status code of the response indicates an HTTP (i.e., 400-599) error.
        /// </summary>
        /// <param name="apiResponse"></param>
        /// <returns></returns>
        public static bool IsErrorStatusCode(this ApiResponse apiResponse) => apiResponse.StatusCode >= 400 && apiResponse.StatusCode <= 599;
    }
}
