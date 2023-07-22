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
        public string Arguments { get; set; }

        public FunctionCall(string name, string arguments)
        {
            Name = name;
            Arguments = arguments;
        }
    }
}
