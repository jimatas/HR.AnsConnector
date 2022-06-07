using System.Text.Json.Serialization;

namespace HR.AnsConnector.Features.Studies
{
    public class Study
    {
        /// <summary>
        /// The Ans-generated unique id.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Created at timestamp.
        /// </summary>
        /// <example>2022-05-19T10:50:35.973+02:00</example>
        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Last updated at timestamp.
        /// </summary>
        /// <example>2022-05-19T10:50:35.973+02:00</example>
        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// The name of the study
        /// </summary>
        /// <example>Study 1</example>
        public string? Name { get; set; }

        /// <summary>
        /// Identifies the department the study belongs to.
        /// </summary>
        [JsonPropertyName("department_id")]
        public int? DepartmentId { get; set; }

        /// <summary>
        /// An external id to reference the study.
        /// </summary>
        /// <example>Ext1</example>
        [JsonPropertyName("external_id")]
        public string? ExternalId { get; set; }

        /// <inheritdoc/>
        public override string ToString() => $"{nameof(Study)} with {nameof(ExternalId)} '{ExternalId}'";
    }
}
