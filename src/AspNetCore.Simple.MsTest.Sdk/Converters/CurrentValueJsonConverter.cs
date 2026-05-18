using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AspNetCore.Simple.MsTest.Sdk.Converters
{
    /// <summary>
    /// Custom JSON converter that handles the 'currentValue' property in error responses.
    /// When the backend double-serializes request content (e.g., "" becomes "\"\""),
    /// this converter un-escapes it to make responses more readable.
    /// </summary>
    internal sealed class CurrentValueJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(JObject) || objectType == typeof(JToken);
        }

        public override object? ReadJson(JsonReader reader,
                                         Type objectType,
                                         object? existingValue,
                                         JsonSerializer serializer)
        {
            // Normal deserialization - no modification needed
            return JToken.Load(reader);
        }

        public override void WriteJson(JsonWriter writer,
                                       object? value,
                                       JsonSerializer serializer)
        {
            if (value is JToken token)
            {
                ProcessToken(token);
                token.WriteTo(writer);
            }
        }

        private static void ProcessToken(JToken token)
        {
            if (token is JObject obj)
            {
                ProcessObject(obj);
            }
            else if (token is JArray array)
            {
                foreach (var item in array)
                {
                    ProcessToken(item);
                }
            }
        }

        private static void ProcessObject(JObject obj)
        {
            foreach (var property in obj.Properties().ToList())
            {
                // Handle 'currentValue' property specifically
                if (property.Name == "currentValue" && property.Value.Type == JTokenType.String)
                {
                    var stringValue = property.Value.ToString();

                    // Try to parse the string as JSON to get the actual value
                    // This handles cases where the backend serializes JSON primitives as strings:
                    // - "\"\"" becomes "" (empty string)
                    // - "[]" becomes [] (empty array)
                    // - "{}" becomes {} (empty object)
                    // - "null" becomes null
                    try
                    {
                        var parsed = JsonConvert.DeserializeObject(stringValue);
                        if (parsed != null)
                        {
                            // Check if it's a simple JSON value that was stringified
                            var token = JToken.FromObject(parsed);

                            // Replace string representation with actual JSON value
                            property.Value = token;
                        }
                        else if (stringValue == "null")
                        {
                            // Handle explicit null string
                            property.Value = JValue.CreateNull();
                        }
                    }
                    catch (JsonException)
                    {
                        // If parsing fails, keep the original string value
                        // This means it's genuinely a string, not a stringified JSON value
                    }
                }

                // Recursively process nested objects and arrays
                ProcessToken(property.Value);
            }
        }
    }
}