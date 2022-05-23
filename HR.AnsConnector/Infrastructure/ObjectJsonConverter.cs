using System.Text.Json;
using System.Text.Json.Serialization;

namespace HR.AnsConnector.Infrastructure
{
    /// <summary>
    /// A JSON converter that can read System.Object-typed values.
    /// </summary>
    public class ObjectJsonConverter : JsonConverter<object>
    {
        /// <inheritdoc/>
        public override object? Read(ref Utf8JsonReader jsonReader, Type typeToConvert, JsonSerializerOptions jsonOptions)
        {
            var tokenType = jsonReader.TokenType;
            if (tokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (tokenType == JsonTokenType.Number)
            {
                if (jsonReader.TryGetInt32(out var int32Value))
                {
                    return int32Value;
                }

                if (jsonReader.TryGetInt64(out var int64Value))
                {
                    return int64Value;
                }

                if (jsonReader.TryGetDouble(out var doubleValue))
                {
                    return doubleValue;
                }
            }

            if (tokenType == JsonTokenType.True || tokenType == JsonTokenType.False)
            {
                return jsonReader.GetBoolean();
            }

            if (tokenType == JsonTokenType.String)
            {
                return jsonReader.GetString();
            }

            using var jsonDocument = JsonDocument.ParseValue(ref jsonReader);
            return jsonDocument.RootElement.Clone();
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter jsonWriter, object value, JsonSerializerOptions jsonOptions)
            => throw new NotSupportedException("Directly writing System.Object is not supported.");
    }
}
