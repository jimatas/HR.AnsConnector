using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HR.AnsConnector.Features.Courses
{
    public class Course
    {
        /// <summary>
        /// The Ans-generated unique id.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Created at timestamp.
        /// </summary>
        /// <example>2022-05-19T10:49:19.125+02:00</example>
        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Last updated at timestamp.
        /// </summary>
        /// <example>2022-05-19T10:49:19.125+02:00</example>
        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// The name of the course
        /// </summary>
        /// <example>Course name</example>
        [Required]
        public string? Name { get; set; }

        /// <summary>
        /// The year in which the school year started.
        /// </summary>
        /// <example>2020</example>
        public int? Year { get; set; }

        /// <summary>
        /// Code used to refer to the course
        /// </summary>
        /// <example>Course code 1</example>
        [JsonPropertyName("course_code")]
        public string? CourseCode { get; set; }

        /// <summary>
        /// Allows a student to enroll themselves in the course if set to true.
        /// </summary>
        [JsonPropertyName("self_enroll")]
        public bool SelfEnroll { get; set; }

        /// <summary>
        /// An external id to reference the course.
        /// </summary>
        /// <example>Ext1</example>
        [JsonPropertyName("external_id")]
        public string? ExternalId { get; set; }

        /// <summary>
        /// An array of study ids that the course belongs to.
        /// </summary>
        /// <example>[ 1, 2 ]</example>
        [JsonPropertyName("study_ids")]
        public IEnumerable<int> StudyIds { get; set; } = Enumerable.Empty<int>();

        /// <summary>
        /// Defines if a course has been soft deleted.
        /// </summary>
        [JsonPropertyName("trashed")]
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// If the course has been soft deleted, the timestamp of deletion.
        /// </summary>
        [JsonPropertyName("trashed_at")]
        public DateTime? DeletedAt { get; set; }

        /// <inheritdoc/>
        public override string ToString() => $"{nameof(Course)} with {nameof(ExternalId)} '{ExternalId}'";
    }
}
