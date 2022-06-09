using System.ComponentModel.DataAnnotations;

namespace HR.AnsConnector.Infrastructure
{
    public class ApiSettings
    {
        /// <summary>
        /// Base URI of the service, including trailing slash.
        /// Default value: https://ans.app/api/v2/
        /// </summary>
        [Required]
        [RegularExpression("/$", ErrorMessage = "The field {0} must include a trailing slash.")]
        public string BaseUri { get; set; } = "https://ans.app/api/v2/";

        [Required]
        public string? BearerToken { get; set; }

        [Required]
        public int? TenantId { get; set; }
    }
}
