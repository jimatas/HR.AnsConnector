namespace HR.AnsConnector.Infrastructure
{
    public class ApiResponse
    {
        /// <summary>
        /// The status code of the response.
        /// </summary>
        public int? StatusCode { get; internal set; }

        /// <summary>
        /// A string describing the status code, such as "OK" or "Not Found".
        /// </summary>
        public string? StatusDescription { get; internal set; }
    }
}
