using System.Text.Json.Serialization;

namespace HR.AnsConnector.Features.Users
{
    public class UserRecord : User
    {
        [JsonIgnore]
        public int? EventId { get; set; }

        [JsonIgnore]
        public string? Action { get; set; }
    }
}
