namespace HR.AnsConnector.Infrastructure
{
    public static class ApiResponseExtensions
    {
        public static string GetValidationErrorsAsSingleMessage<T>(this ApiResponse<T> apiResponse)
            => string.Join(Environment.NewLine, apiResponse.ValidationErrors.Select(error => $"{error.Key}: {string.Join(", ", error.Value)}"));
    }
}
