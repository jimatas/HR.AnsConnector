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

        /// <summary>
        /// Returns a string that contains both the status code and description of the response, prepended with the string "HTTP".
        /// For example, "HTTP 404 - Not Found".
        /// </summary>
        /// <param name="apiResponse"></param>
        /// <returns></returns>
        public static string GetStatusMessage(this ApiResponse apiResponse) => $"HTTP {apiResponse.StatusCode} - {apiResponse.StatusDescription}";

        /// <summary>
        /// Produces a single flattened error message given the individual validation errors, if any, that were returned in the response.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiResponse"></param>
        /// <returns></returns>
        public static string GetValidationErrorsAsSingleMessage<T>(this ApiResponse<T> apiResponse)
            => string.Join(Environment.NewLine, apiResponse.ValidationErrors.SelectMany(error => error.Value.Select(value => $"{error.Key}: {value}")));
    }
}
