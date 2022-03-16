using System.Text.Json.Serialization;

namespace HR.AnsConnector.Features.Users
{
    public class UserRecord : User
    {
        [JsonIgnore]
        public int? EventId { get; set; }

        /// <summary>
        /// Specifies the type of action to perform:
        /// <list type="table">
        ///   <item>
        ///     <term>"c"</term>
        ///     <description>Create a new student.</description>
        ///   </item>
        ///   <item>
        ///     <term>"u"</term>
        ///     <description>Update an existing student.</description>
        ///   </item>
        ///   <item>
        ///     <term>"d"</term>
        ///     <description>(Soft-)delete a student.</description>
        ///   </item>
        /// </list>
        /// </summary>
        [JsonIgnore]
        public string? Action { get; set; }
    }
}
