namespace HR.AnsConnector.Infrastructure
{
    public class ApiSettings
    {
        /// <summary>
        /// Base URI of the service, including trailing slash.
        /// Default value: https://ans.app/api/v2/
        /// </summary>
        public string BaseUri { get; set; } = "https://ans.app/api/v2/";
        public string? BearerToken { get; set; }
        public int? TenantId { get; set; }
    }
}
