using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HR.AnsConnector.Features.Users
{
    public class UserSearchCriteria
    {
        /// <summary>
        /// Search user(s) by external id field.
        /// </summary>
        [JsonPropertyName("external_id")]
        public string? ExternalId { get; set; }

        /// <summary>
        /// Search user(s) by email field.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Search user(s) by student number field.
        /// </summary>
        [JsonPropertyName("student_number")]
        public string? StudentNumber { get; set; }

        public string ToQueryString(JsonNamingPolicy? propertyNamingPolicy = null)
        {
            StringBuilder queryStringBuilder = new();

            foreach (var property in GetType().GetProperties())
            {
                var propertyValue = property.GetValue(this)?.ToString();
                if (!string.IsNullOrEmpty(propertyValue))
                {
                    var propertyName = property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name
                        ?? propertyNamingPolicy?.ConvertName(property.Name)
                        ?? property.Name;

                    if (queryStringBuilder.Length != 0)
                    {
                        queryStringBuilder.Append(' ');
                    }

                    queryStringBuilder.Append(propertyName).Append(':');

                    var quotePropertyValue = propertyValue.Any(char.IsWhiteSpace);
                    if (quotePropertyValue)
                    {
                        queryStringBuilder.Append('\'');
                    }
                    queryStringBuilder.Append(Uri.EscapeDataString(propertyValue));
                    if (quotePropertyValue)
                    {
                        queryStringBuilder.Append('\'');
                    }
                }
            }

            return queryStringBuilder.ToString();
        }
    }
}
