﻿using System.Text.Json.Serialization;

namespace HR.AnsConnector.Features.Departments
{
    public class DepartmentRecord : Department
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
