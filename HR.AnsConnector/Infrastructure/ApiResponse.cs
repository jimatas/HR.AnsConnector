namespace HR.AnsConnector.Infrastructure
{
    public class ApiResponse
    {
        /// <summary>
        /// The status code of the response.
        /// </summary>
        public int? StatusCode { get; internal set; }

        /// <summary>
        /// The status message of the response.
        /// </summary>
        public string? StatusMessage { get; internal set; }
    }
}
