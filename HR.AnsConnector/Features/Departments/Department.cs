using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HR.AnsConnector.Features.Departments
{
    public class Department
    {
        /// <summary>
        /// The Ans-generated unique id.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Created at timestamp.
        /// </summary>
        /// <example>2022-03-22T16:16:51.952+01:00</example>
        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Last updated at timestamp.
        /// </summary>
        /// <example>2022-03-22T16:16:51.952+01:00</example>
        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// The name of the department
        /// </summary>
        /// <example>Department 1</example>
        [Required]
        public string? Name { get; set; }

        /// <summary>
        /// An array of user ids that have the operator role that will be added to the department.
        /// </summary>
        /// <example>[ 1, 2, 3 ]</example>
        public IEnumerable<int> OperatorIds { get; set; } = Enumerable.Empty<int>();

        /// <summary>
        /// An external id to reference the department.
        /// </summary>
        /// <example>Ext1</example>
        [JsonPropertyName("external_id")]
        public string? ExternalId { get; set; }
    }
}
