using System.Text.Json.Serialization;

namespace HR.AnsConnector.Features.Users
{
    /// <summary>
    /// Extends the <see cref="User"/> class by adding metadata properties such as those for event correlation and which action to perform on the data in Ans.
    /// </summary>
    public class UserRecord : User
    {
        [JsonIgnore]
        public int? EventId { get; set; }

        /// <summary>
        /// Specifies the type of action to perform:
        /// <list type="table">
        ///   <item>
        ///     <term>"c"</term>
        ///     <description>Create a new entry.</description>
        ///   </item>
        ///   <item>
        ///     <term>"u"</term>
        ///     <description>Update an existing entry.</description>
        ///   </item>
        ///   <item>
        ///     <term>"d"</term>
        ///     <description>(Soft-)delete an entry.</description>
        ///   </item>
        /// </list>
        /// </summary>
        [JsonIgnore]
        public string? Action { get; set; }
    }
}
