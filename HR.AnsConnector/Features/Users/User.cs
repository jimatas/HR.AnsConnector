using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HR.AnsConnector.Features.Users
{
    public class User
    {
        public int? Id { get; set; }

        /// <summary>
        /// Created at timestamp.
        /// </summary>
        /// <example>2022-02-21T14:08:47.277+01:00</example>
        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Last updated at timestamp.
        /// </summary>
        /// <example>2022-02-21T14:08:47.498+01:00</example>
        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// The first name of a user.
        /// </summary>
        /// <example>John</example>
        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        /// <summary>
        /// The middle name of a user.
        /// </summary>
        /// <example>van</example>
        [JsonPropertyName("middle_name")]
        public string? MiddleName { get; set; }

        /// <summary>
        /// The last name of a user.
        /// </summary>
        /// <example>Doe</example>
        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }

        /// <summary>
        /// Displays whether a user is active, if false the user will be given a screen that their account is disabled.
        /// </summary>
        [JsonPropertyName("active")]
        public bool? IsActive { get; set; }

        /// <summary>
        /// The email tied to the user account.
        /// </summary>
        /// <example>john_doe@ans.app</example>
        [Required]
        public string? Email { get; set; }

        /// <summary>
        /// A unique set of numbers which is required for students (validated against the length set on the school).
        /// </summary>
        /// <example>123456</example>
        [JsonPropertyName("student_number")]
        public string? StudentNumber { get; set; }

        /// <summary>
        /// Defines if a user has been soft deleted.
        /// Default: <c>false</c>
        /// </summary>
        [JsonPropertyName("trashed")]
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// Built-in role.
        /// </summary>
        public UserRole? Role { get; set; }

        /// <summary>
        /// A custom role id when using custom defined roles.
        /// </summary>
        /// <example>1</example>
        [JsonPropertyName("role_id")]
        public int? RoleId { get; set; }

        /// <summary>
        /// An array of department ids which the user will be added to.
        /// This field is ignored for any role except operators.
        /// </summary>
        /// <example>[ 7 ]</example>
        [JsonPropertyName("department_ids")]
        public IEnumerable<int> DepartmentIds { get; set; } = Enumerable.Empty<int>();

        /// <summary>
        /// A unique id for the user which is used for SSO.
        /// </summary>
        /// <example>UID</example>
        [JsonPropertyName("uid")]
        public string? UniqueId { get; set; }

        /// <summary>
        /// An external id to reference the user.
        /// </summary>
        /// <example>Ext1</example>
        [JsonPropertyName("external_id")]
        public string? ExternalId { get; set; }

        /// <inheritdoc/>
        public override string ToString() => $"{nameof(User)} with {nameof(ExternalId)} '{ExternalId}'";
    }
}
