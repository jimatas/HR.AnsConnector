namespace HR.AnsConnector.Infrastructure
{
    public static class ApiResponseExtensions
    {
        /// <summary>
        /// Produces a single flattened error message given the validation errors, if any, that were returned by the server.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiResponse"></param>
        /// <returns></returns>
        public static string GetValidationErrorsAsSingleMessage<T>(this ApiResponse<T> apiResponse)
            => string.Join(Environment.NewLine, apiResponse.ValidationErrors.Select(error => $"{error.Key}: {string.Join(", ", error.Value)}"));
    }
}
