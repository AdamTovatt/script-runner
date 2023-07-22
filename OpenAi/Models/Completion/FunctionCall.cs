using OpenAi.Converters;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace OpenAi.Models.Completion
{
    /// <summary>
    /// Represents a function call that OpenAi wants to do
    /// </summary>
    public class FunctionCall
    {
        /// <summary>
        /// The name of the function that OpenAi wants to call
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The arguments to pass to the function, as a json string that can be deserialized into an object
        /// </summary>
        [JsonPropertyName("arguments")]
        [JsonConverter(typeof(ArgumentsConverter))]
        public Dictionary<string, JsonNode>? Arguments { get; set; }

        public FunctionCall(string name, Dictionary<string, JsonNode>? arguments)
        {
            Name = name;
            Arguments = arguments;
        }

        public static Dictionary<string, object>? DeserializeArgumentsJson(string json)
        {
            JsonSerializerOptions options = new JsonSerializerOptions() { UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode };
            Dictionary<string, object>? result = JsonSerializer.Deserialize<Dictionary<string, object>>(json, options);
            return result;
        }
    }
}
