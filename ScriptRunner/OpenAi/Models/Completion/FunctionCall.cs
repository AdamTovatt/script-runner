using ScriptRunner.OpenAi.Converters;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ScriptRunner.OpenAi.Models.Completion
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

        /// <summary>
        /// Will create a function call from a given name of a function
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <returns></returns>
        public static FunctionCall FromName(string name)
        {
            return new FunctionCall(name, null);
        }
    }
}
